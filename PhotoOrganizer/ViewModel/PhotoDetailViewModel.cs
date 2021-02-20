using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoDetailViewModel : DetailViewModelBase, IPhotoDetailViewModel
    {
        private PhotoWrapper _photo;
        private PeopleWrapper _selectedPeople;
        private IYearLookupDataService _yearLookupDataService;
        private IPhotoRepository _photoRepository;

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

        public PhotoDetailViewModel(IPhotoRepository photoRepository, 
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IYearLookupDataService yearLookupDataService
            ) : base(eventAggregator, messageDialogService)
        {
            _photoRepository = photoRepository;
            _yearLookupDataService = yearLookupDataService;

            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Subscribe(AfterCollectionSaved);

            AddPeopleCommand = new DelegateCommand(OnAddPeopleExecute);
            RemovePeopleCommand = new DelegateCommand(OnRemovePeopleExecute, OnRemovePeopleCanExecute);

            Years = new ObservableCollection<LookupItem>();
            Peoples = new ObservableCollection<PeopleWrapper>();
        }

        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if(args.ViewModelName == nameof(YearDetailViewModel))
            {
                await LoadYearLookupAsync();
            }
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

        public override async Task LoadAsync(int photoId)
        {
            var photo = photoId > 0
                ? await _photoRepository.GetByIdAsync(photoId)
                : CreateNewPhoto();

            Id = photoId;

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
                if(e.PropertyName == nameof(Photo.Title))
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Photo.Id == 0)
            {
                Photo.Title = "";
                Photo.FullPath = "";
            }

            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Photo.Title}";
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

        // TODO: refactor if PhotoService layer has been created this should be moved there
        private Photo CreateNewPhoto()
        {
            var photo = new Photo();
            _photoRepository.Add(photo);
            return photo;
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync, 
                () => 
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.FullPath}");
                });           
        }

        protected override bool OnSaveCanExecute()
        {
            return Photo != null 
                && !Photo.HasErrors 
                && Peoples.All(p => !p.HasErrors)
                && HasChanges;
        }

        protected override async void OnDeleteExecute()
        {
            if(await _photoRepository.HasAlbums(Photo.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync($"{Photo.Title} can't be deleted as it is part of at least one album.");
                return;
            }

            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete {Photo.Title}?", "Question");
            if(result == MessageDialogResult.Ok)
            {
                _photoRepository.Remove(Photo.Model);
                await _photoRepository.SaveAsync();
                RaiseDetailDeletedEvent(Photo.Id);                
            }
        }
    }
}
