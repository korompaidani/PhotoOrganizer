using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoDetailViewModel : ViewModelBase, IPhotoDetailViewModel
    {
        private PhotoWrapper _photo;
        private IPhotoRepository _photoRepository;
        private IEventAggregator _eventAggregator;
        private PeopleWrapper _selectedPeople;
        private IMessageDialogService _messageDialogService;
        private IYearLookupDataService _yearLookupDataService;
        private bool _hasChanges;

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddPeopleCommand { get; }
        public ICommand RemovePeopleCommand { get; }

        public ObservableCollection<LookupItem> Years { get; }
        public ObservableCollection<PeopleWrapper> Peoples { get; }

        public PhotoWrapper Photo
        { 
            get { return _photo; }
            set 
            {
                _photo = value;
                OnPropertyChanged();                
            }
        }

        public PeopleWrapper SelectedPeople {
            get { return _selectedPeople; } 
            set 
            {
                _selectedPeople = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePeopleCommand).RaiseCanExecuteChanged();
            }
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            private set 
            { 
                if(_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public PhotoDetailViewModel(IPhotoRepository photoRepository, 
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IYearLookupDataService yearLookupDataService
            )
        {
            _photoRepository = photoRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _yearLookupDataService = yearLookupDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            AddPeopleCommand = new DelegateCommand(OnAddPeopleExecute);
            RemovePeopleCommand = new DelegateCommand(OnRemovePeopleExecute, OnRemovePeopleCanExecute);

            Years = new ObservableCollection<LookupItem>();
            Peoples = new ObservableCollection<PeopleWrapper>();
        }

        private bool OnRemovePeopleCanExecute()
        {
            return SelectedPeople != null;
        }

        private void OnRemovePeopleExecute()
        {
            SelectedPeople.PropertyChanged -= PeopleWrapper_PropertyChanged;
            _photoRepository.RemovePeople(SelectedPeople.Model);
            Peoples.Remove(SelectedPeople);
            SelectedPeople = null;
            HasChanges = _photoRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPeopleExecute()
        {
            var newPeople = new PeopleWrapper(new People());
            newPeople.PropertyChanged += PeopleWrapper_PropertyChanged;
            Peoples.Add(newPeople);
            Photo.Model.Peoples.Add(newPeople.Model);
            newPeople.FirstName = "";
        }

        public async Task LoadAsync(int? photoId)
        {
            var photo = photoId.HasValue
                ? await _photoRepository.GetByIdAsync(photoId.Value)
                : CreateNewPhoto();
            
            InitializePhoto(photo);

            InitializePeople(photo.Peoples);

            await LoadYearLookupAsync();
        }

        private void InitializePeople(ICollection<People> peoples)
        {
            foreach(var people in Peoples)
            {
                people.PropertyChanged -= PeopleWrapper_PropertyChanged;
            }
            Peoples.Clear();
            foreach(var people in peoples)
            {
                var wrapper = new PeopleWrapper(people);
                Peoples.Add(wrapper);
                wrapper.PropertyChanged += PeopleWrapper_PropertyChanged;
            }
        }

        private void PeopleWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _photoRepository.HasChanges();
            }
            if(e.PropertyName == nameof(PeopleWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void InitializePhoto(Photo photo)
        {
            Photo = new PhotoWrapper(photo);
            Photo.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _photoRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Photo.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Photo.Id == 0)
            {
                Photo.Title = "";
                Photo.FullPath = "";
            }
        }

        private async Task LoadYearLookupAsync()
        {
            Years.Clear();
            Years.Add(new NullLookupItem { DisplayMemberItem = "-" });
            var lookup = await _yearLookupDataService.GetYearLookupAsync();
            foreach (var lookupItem in lookup)
            {
                Years.Add(lookupItem);
            }
        }

        private Photo CreateNewPhoto()
        {
            var photo = new Photo();
            _photoRepository.Add(photo);
            return photo;
        }

        private async void OnSaveExecute()
        {
            await _photoRepository.SaveAsync();
            HasChanges = _photoRepository.HasChanges();
            _eventAggregator.GetEvent<AfterPhotoSavedEvent>().Publish(
                new AfterPhotoSavedEventArgs
                {
                    Id = Photo.Id,
                    Title = $"{Photo.Title}"
                });
        }

        private bool OnSaveCanExecute()
        {
            return Photo != null 
                && !Photo.HasErrors 
                && Peoples.All(p => !p.HasErrors)
                && HasChanges;
        }

        private async void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete {Photo.Title}?", "Question");
            if(result == MessageDialogResult.Ok)
            {
                _photoRepository.Remove(Photo.Model);
                await _photoRepository.SaveAsync();
                _eventAggregator.GetEvent<AfterPhotoDeleteEvent>().Publish(Photo.Id);
            }
        }
    }
}
