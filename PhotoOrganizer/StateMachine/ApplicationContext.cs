using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System;
using System.Collections.Generic;

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
        private List<IDetailViewModel> _openedPhotoDetailViewModels;
        private List<IDetailViewModel> _openedAlbumDetailViewModels;
        private List<IDetailViewModel> _openedLocationDetailViewModels;

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
