using PhotoOrganizer.Common;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Helpers;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.StateMachine
{
    public class ApplicationContext
    {
        private IMessageDialogService _messageDialogService;
        private IBulkAttributeSetterService _bulkAttributeSetter;
        private IEventAggregator _eventAggregator;
        private IPhotoRepository _photorepository;
        private ILocationRepository _locationRepository;
        private IAlbumRepository _albumRepository;
        private MessageDialogResult _internalAnswer;

        private List<IDetailViewModel> _openedPhotoDetailViewModels;
        private List<IDetailViewModel> _openedAlbumDetailViewModels;
        private List<IDetailViewModel> _openedLocationDetailViewModels;
        private List<KeyValuePair<ErrorTypes, string>> _errorMessages;

        public ApplicationContext(
            IMessageDialogService messageDialogService,
            IBulkAttributeSetterService bulkAttributeSetter,
            IEventAggregator eventAggregator,
            IPhotoRepository photoRepository, 
            ILocationRepository locationRepository, 
            IAlbumRepository albumRepository)
        {
            _messageDialogService = messageDialogService;
            _bulkAttributeSetter = bulkAttributeSetter;
            _eventAggregator = eventAggregator;
            _photorepository = photoRepository;
            _locationRepository = locationRepository;
            _albumRepository = albumRepository;

            _openedPhotoDetailViewModels = new List<IDetailViewModel>();
            _openedAlbumDetailViewModels = new List<IDetailViewModel>();
            _openedLocationDetailViewModels = new List<IDetailViewModel>();
            _errorMessages = new List<KeyValuePair<ErrorTypes, string>>();

            _eventAggregator.GetEvent<WriteAllMetadataEvent>()
                .Subscribe(WriteAllMetadata);
            _eventAggregator.GetEvent<DisplayMessageEvent>()
                .Subscribe(DisplayMessage);
            _eventAggregator.GetEvent<SaveAllDetailViewEvent>()
                .Subscribe(SaveAllDetailView);
            _eventAggregator.GetEvent<ErrorEvent>()
                .Subscribe(GetAnError);
        }

        public void RegisterOpenedDetailView(IDetailViewModel detailViewModel, string modelViewType)
        {
            if(modelViewType == nameof(PhotoDetailViewModel))
            {
                _openedPhotoDetailViewModels.Add(detailViewModel);
            }
            if (modelViewType == nameof(AlbumDetailViewModel))
            {
                _openedAlbumDetailViewModels.Add(detailViewModel);
            }
            if (modelViewType == nameof(LocationDetailViewModel))
            {
                _openedLocationDetailViewModels.Add(detailViewModel);
            }
        }

        public void UnRegisterOpenedDetailView(IDetailViewModel detailViewModel, string modelViewType)
        {
            if (modelViewType == nameof(PhotoDetailViewModel))
            {
                _openedPhotoDetailViewModels.Remove(detailViewModel);
            }
            if (modelViewType == nameof(AlbumDetailViewModel))
            {
                _openedAlbumDetailViewModels.Remove(detailViewModel);
            }
            if (modelViewType == nameof(LocationDetailViewModel))
            {
                _openedLocationDetailViewModels.Remove(detailViewModel);
            }
        }


        public void AddErrorMessage(ErrorTypes errorType, string errorMessage)
        {
            _errorMessages.Add(new KeyValuePair<ErrorTypes, string>(errorType, errorMessage));
        }

        public async Task<List<KeyValuePair<int, PhotoDetailInfo>>> SaveAllTab(bool isForceSaveAll = false)
        {
            var tabIds = new List<KeyValuePair<int, PhotoDetailInfo>>();
            await SaveAllOpenedDetailView(false, isForceSaveAll);
            if(_internalAnswer == MessageDialogResult.Cancel)
            {
                return tabIds;
            }

            foreach(var photoTab in _openedPhotoDetailViewModels)
            {
                var photo = (PhotoDetailViewModel)photoTab;
                var detailInfo = new PhotoDetailInfo
                {
                    Id = photoTab.Id,
                    Title = photo.Photo.Title,
                    FullFilePath = photo.Photo.FullPath,
                    ColorFlag = ColorMap.Map[photo.Photo.ColorFlag],
                    ViewModelName = nameof(PhotoDetailViewModel),
                    IsShelveRelevant = false
                };

                var keyValues = new KeyValuePair<int, PhotoDetailInfo>(photoTab.Id, detailInfo);
                tabIds.Add(keyValues);
            }

            foreach (var albumTab in _openedAlbumDetailViewModels)
            {
                var album = (AlbumDetailViewModel)albumTab;
                var detailInfo = new PhotoDetailInfo
                {
                    Id = albumTab.Id,
                    Title = album.Title,
                    ViewModelName = nameof(AlbumDetailViewModel)
                };

                var keyValues = new KeyValuePair<int, PhotoDetailInfo>(albumTab.Id, detailInfo);
                tabIds.Add(keyValues);
            }

            foreach (var locationTab in _openedLocationDetailViewModels)
            {
                var detailInfo = new PhotoDetailInfo
                {
                    Id = locationTab.Id,
                    ViewModelName = nameof(LocationDetailViewModel)
                };

                var keyValues = new KeyValuePair<int, PhotoDetailInfo>(locationTab.Id, detailInfo);
                tabIds.Add(keyValues);
            }

            return tabIds;
        }

        public async Task<bool> SaveAllOpenedDetailView(bool isOnClosing = true, bool isForceSaveAll = false)
        {
            bool canClose = false;
            MessageDialogResult result = MessageDialogResult.Ok;

            if (_openedPhotoDetailViewModels.Count > 0 ||
                _openedAlbumDetailViewModels.Count > 0 ||
                _openedLocationDetailViewModels.Count > 0)
            {

                if (!isForceSaveAll)
                {
                    result = await _messageDialogService.ShowSaveSaveAllDiscardDiscardAllDialogAsync();
                }
                else
                {
                    result = MessageDialogResult.SaveAll;
                }

                if (result != MessageDialogResult.DiscardAll && result != MessageDialogResult.Cancel)
                {
                    await SaveOpenedDetailViews(_openedPhotoDetailViewModels, result);
                    await SaveOpenedDetailViews(_openedAlbumDetailViewModels, result);
                    await SaveOpenedDetailViews(_openedLocationDetailViewModels, result);
                    canClose = true;
                    result = _internalAnswer;
                }
            }
            
            if (result != MessageDialogResult.Cancel && isOnClosing)
            {
                System.Windows.Application.Current.Shutdown();
            }

            return canClose;
        }

        private async Task SaveOpenedDetailViews(List<IDetailViewModel> openedDetailView, MessageDialogResult answer)
        {
            foreach (var detailView in openedDetailView)
            {
                try
                {                    
                    if (answer == MessageDialogResult.Save || answer == MessageDialogResult.SaveAll)
                    {

                        if (detailView.HasChanges)
                        {
                            await detailView.SaveChanges(true);
                        }
                    }
                    if (answer != MessageDialogResult.DiscardAll && answer != MessageDialogResult.SaveAll)
                    {
                        var detailViewModelBase = detailView as DetailViewModelBase;
                        string title = null;
                        if (detailViewModelBase != null)
                        {
                            title = detailViewModelBase.Title;
                        }

                        _internalAnswer = await _messageDialogService.ShowSaveSaveAllDiscardDiscardAllDialogAsync(title);
                        answer = _internalAnswer;
                    }
                    if(answer == MessageDialogResult.Cancel)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _errorMessages.Add(new KeyValuePair<ErrorTypes, string>(ErrorTypes.DetailViewClosingError, ex.InnerException.Message));
                }
            }
        }

        private void WriteAllMetadata(WriteAllMetadataEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void DisplayMessage(DisplayMessageEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void SaveAllDetailView(SaveAllDetailViewEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void GetAnError(ErrorEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
