using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoDetailViewModel : ViewModelBase, IPhotoDetailViewModel
    {
        private PhotoWrapper _photo;        
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IYearLookupDataService _yearLookupDataService;
        private bool _hasChanges;

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ObservableCollection<LookupItem> Years { get; }

        public PhotoWrapper Photo
        { 
            get { return _photo; }
            set 
            {
                _photo = value;
                OnPropertyChanged();                
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
            ) : base(photoRepository)
        {            
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _yearLookupDataService = yearLookupDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);

            Years = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync(int? photoId)
        {
            var photo = photoId.HasValue
                ? await _photoRepository.GetByIdAsync(photoId.Value)
                : CreateNewPhoto();
            
            InitializePhoto(photo);
            await LoadYearLookupAsync();
        }

        private void InitializePhoto(Photo photo)
        {
            Photo = new PhotoWrapper(photo, _photoRepository);
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
            return Photo != null && !Photo.HasErrors && HasChanges;
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
