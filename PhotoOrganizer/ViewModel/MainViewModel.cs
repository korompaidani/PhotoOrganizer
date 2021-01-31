﻿using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IPhotoDetailViewModel _photoDetailViewModel;
        private Func<IPhotoDetailViewModel> _photoDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;
        private IDirectoryReaderWrapperService _directoryReaderWrapperService;
        private IEventAggregator _eventAggregator;        

        public ICommand CreateNewPhotoCommand { get; }
        public ICommand CreatePhotosFromLibraryCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }
        public IPhotoDetailViewModel PhotoDetailViewModel 
        { 
            get 
            {
                return _photoDetailViewModel;
            } 
            private set 
            {
                _photoDetailViewModel = value;
                OnPropertyChanged();
            } 
        }

        public MainViewModel(
            IPhotoRepository photoRepository,
            INavigationViewModel navigationViewModel, 
            Func<IPhotoDetailViewModel> photoDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IDirectoryReaderWrapperService directoryReaderWrapperService) : base(photoRepository)
        {
            NavigationViewModel = navigationViewModel;
            _photoDetailViewModelCreator = photoDetailViewModelCreator;
            _messageDialogService = messageDialogService;
            _directoryReaderWrapperService = directoryReaderWrapperService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Subscribe(OnOpenPhotoDetailView);
            _eventAggregator.GetEvent<AfterPhotoDeleteEvent>().Subscribe(AfterPhotoDeleted);

            CreateNewPhotoCommand = new DelegateCommand(OnCreateNewPhotoExecute);
            CreatePhotosFromLibraryCommand = new DelegateCommand(OnCreatePhotosFromLibraryExecute);
        }        

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenPhotoDetailView(int? photoId)
        {
            if(PhotoDetailViewModel != null && PhotoDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("Are you sure to leave this form? Changes will lost.", "Question");
                if(result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            PhotoDetailViewModel = _photoDetailViewModelCreator();
            await PhotoDetailViewModel.LoadAsync(photoId);
        }

        private void OnCreateNewPhotoExecute()
        {
            OnOpenPhotoDetailView(null);
        }

        private async void OnCreatePhotosFromLibraryExecute()
        {
            // TODO:
            // 1.        Detect that database has entries
            // 2.        if yes: ask to save --> and if yes than save
            // 3.        Delete all data from photo table
            // 3. DONE   Read data from library
            // 4.        show progressbar during load

            var result = _messageDialogService.ShowOkCancelDialog("This operation will erase all previous data from Database. Are you sure to load new library data?", "Question");
            if (result == MessageDialogResult.Cancel)
            {
                return;
            }
            else
            {
                await _photoRepository.TruncatePhotoTable();
            }

            await LoadAllFromLibraryAsync();
        }


        public async Task LoadAllFromLibraryAsync()
        {
            foreach (var photo in _directoryReaderWrapperService.ConvertFileNamesToPhotos())
            {
                CreateNewPhoto(photo);
            }
            await _photoRepository.SaveAsync();
            await LoadAsync();
        }

        private void AfterPhotoDeleted(int photoId)
        {
            PhotoDetailViewModel = null;
        }
    }
}
