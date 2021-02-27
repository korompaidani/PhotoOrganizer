using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class AlbumDetailViewModel : DetailViewModelBase, IAlbumDetailViewModel
    {
        private IAlbumRepository _albumRepository;
        private AlbumWrapper _selectedAlbum;
        
        private Photo _selectedAvailablePhoto;
        private Photo _selectedAddedPhoto;
        private List<Photo> _allPhotos;

        public ObservableCollection<Photo> AddedPhotos { get; }
        public ObservableCollection<Photo> AvailablePhotos { get; }
        public ICommand AddPhotoCommand { get; }
        public ICommand RemovePhotoCommand { get; }

        public Photo SelectedAvailablePhoto
        {
            get { return _selectedAvailablePhoto; }
            set
            {
                _selectedAvailablePhoto = value;
                OnPropertyChanged();
                ((DelegateCommand)AddPhotoCommand).RaiseCanExecuteChanged();
            }
        }

        public Photo SelectedAddedPhoto
        {
            get { return _selectedAddedPhoto; }
            set
            {
                _selectedAddedPhoto = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhotoCommand).RaiseCanExecuteChanged();
            }
        }

        public AlbumWrapper Album
        {
            get { return _selectedAlbum; }
            set
            {
                _selectedAlbum = value;
                OnPropertyChanged();
            }
        }

        public AlbumDetailViewModel(IAlbumRepository albumRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService            
            ) : base(eventAggregator, messageDialogService)
        {
            _albumRepository = albumRepository;
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            AddedPhotos = new ObservableCollection<Photo>();
            AvailablePhotos = new ObservableCollection<Photo>();
            AddPhotoCommand = new DelegateCommand(OnAddPhotoExecute, OnAddPhotoCanExecute);
            RemovePhotoCommand = new DelegateCommand(OnRemovePhotoExecute, OnRemovePhotoCanExecute);
        }

        public async override Task LoadAsync(int albumId)
        {
            var album = albumId > 0
                ? await _albumRepository.GetByIdAsync(albumId)
                : CreateNewAlbum();

            Id = albumId;

            InitializeAlbum(album);

            _allPhotos = await _albumRepository.GetAllPhotoAsync();

            SetupPicklist();
        }        

        protected async override void OnDeleteExecute()
        {
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete {Album.Title}?", "Question");
            if (result == MessageDialogResult.Ok)
            {
                _albumRepository.Remove(Album.Model);
                await _albumRepository.SaveAsync();
                RaiseDetailDeletedEvent(Album.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Album != null && !Album.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await _albumRepository.SaveAsync();
            HasChanges = _albumRepository.HasChanges();
            Id = Album.Id;
            RaiseDetailSavedEvent(Album.Id, Album.Title);
        }

        private bool OnRemovePhotoCanExecute()
        {
            return SelectedAddedPhoto != null;
        }

        private void OnRemovePhotoExecute()
        {
            var photoToRemove = SelectedAddedPhoto;

            Album.Model.Photos.Remove(photoToRemove);
            AddedPhotos.Remove(photoToRemove);
            AvailablePhotos.Add(photoToRemove);
            HasChanges = _albumRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddPhotoCanExecute()
        {
            return SelectedAvailablePhoto != null;
        }

        private void OnAddPhotoExecute()
        {
            var photoToAdd = SelectedAvailablePhoto;

            Album.Model.Photos.Add(photoToAdd);
            AddedPhotos.Add(photoToAdd);
            AvailablePhotos.Remove(photoToAdd);
            HasChanges = _albumRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(PhotoDetailViewModel))
            {
                await _albumRepository.ReloadPhotoAsync(args.Id);
                _allPhotos = await _albumRepository.GetAllPhotoAsync();
                SetupPicklist();
            }
        }

        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            if (args.ViewModelName == nameof(PhotoDetailViewModel))
            {
                _allPhotos = await _albumRepository.GetAllPhotoAsync();
                SetupPicklist();
            }
        }

        private void InitializeAlbum(object album)
        {
            Album = new AlbumWrapper((Album)album);
            Album.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _albumRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Album.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(Photo.Title))
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if(Album.Id == 0)
            {
                Album.Title = "";
            }

            SetTitle();
        }

        private void SetTitle()
        {
            Title = Album.Title;
        }

        private Album CreateNewAlbum()
        {
            var album = new Album
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };

            _albumRepository.Add(album);
            return album;
        }

        private void SetupPicklist()
        {
            var albumPhotoIds = Album.Model.Photos.Select(p => p.Id).ToList();
            var addedPhotos = _allPhotos.Where(p => albumPhotoIds.Contains(p.Id)).OrderBy(p => p.Title);
            var availablePhotos = _allPhotos.Except(addedPhotos).OrderBy(p => p.Title);

            AddedPhotos.Clear();
            AvailablePhotos.Clear();

            foreach(var addedPhoto in addedPhotos)
            {
                AddedPhotos.Add(addedPhoto);
            }
            foreach (var availablePhoto in availablePhotos)
            {
                AvailablePhotos.Add(availablePhoto);
            }
        }
    }
}
