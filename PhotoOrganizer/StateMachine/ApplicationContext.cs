using PhotoOrganizer.Common;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
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
        private IPhotoMetaWrapperService _photoMetaWrapperService;
        private IEventAggregator _eventAggregator;
        private IPhotoRepository _photorepository;
        private ILocationRepository _locationRepository;
        private IAlbumRepository _albumRepository;
        private bool _isCumulativeAffirmativeDecision = false;

        private List<IDetailViewModel> _openedPhotoDetailViewModels;
        private List<IDetailViewModel> _openedAlbumDetailViewModels;
        private List<IDetailViewModel> _openedLocationDetailViewModels;
        private Dictionary<ErrorTypes, string> _errorMessages;

        public ApplicationContext(
            IMessageDialogService messageDialogService,
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService,
            IEventAggregator eventAggregator,
            IPhotoRepository photoRepository, 
            ILocationRepository locationRepository, 
            IAlbumRepository albumRepository)
        {
            _messageDialogService = messageDialogService;
            _bulkAttributeSetter = bulkAttributeSetter;
            _photoMetaWrapperService = photoMetaWrapperService;
            _eventAggregator = eventAggregator;
            _photorepository = photoRepository;
            _locationRepository = locationRepository;
            _albumRepository = albumRepository;

            _openedPhotoDetailViewModels = new List<IDetailViewModel>();
            _openedAlbumDetailViewModels = new List<IDetailViewModel>();
            _openedLocationDetailViewModels = new List<IDetailViewModel>();
            _errorMessages = new Dictionary<ErrorTypes, string>();

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

        public async Task SaveAllOpenedDetailView()
        {
            // Show dialog and ask cumulative save, discard and single
            _isCumulativeAffirmativeDecision = false;

            await SaveOpenedDetailViews(_openedPhotoDetailViewModels);
            await SaveOpenedDetailViews(_openedAlbumDetailViewModels);
            await SaveOpenedDetailViews(_openedLocationDetailViewModels);

            if (_isCumulativeAffirmativeDecision)
            {
                try
                {
                    await _photorepository.SaveAsync();
                    await _locationRepository.SaveAsync();
                    await _albumRepository.SaveAsync();
                }
                catch (Exception ex)
                {
                    _errorMessages.Add(ErrorTypes.DetailViewClosingError, ex.InnerException.Message);
                }
            }
        }

        private async Task SaveOpenedDetailViews(List<IDetailViewModel> openedDetailView, bool isSave = true)
        {
            if (isSave)
            {
                bool isAffirmativeSaveResult = false;
                foreach (var detailView in openedDetailView)
                {
                    try
                    {
                        if (!_isCumulativeAffirmativeDecision)
                        {
                            // Show dialog
                            isAffirmativeSaveResult = true;

                            if (isAffirmativeSaveResult)
                            {
                                if (detailView.HasChanges)
                                {
                                    await detailView.SaveChanges(true);
                                }
                            }
                        }
                        else
                        {
                            var photoDetail = detailView as PhotoDetailViewModel;
                            if (photoDetail != null)
                            {
                                photoDetail.SetModifiedFlag();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorMessages.Add(ErrorTypes.DetailViewClosingError, ex.InnerException.Message);
                    }
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
