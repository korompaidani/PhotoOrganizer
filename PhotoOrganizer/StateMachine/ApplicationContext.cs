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
        public int ID;
        private IMessageDialogService _messageDialogService;
        private IBulkAttributeSetterService _bulkAttributeSetter;
        private IPhotoMetaWrapperService _photoMetaWrapperService;
        private IEventAggregator _eventAggregator;
        private IPhotoRepository _photorepository;
        private ILocationRepository _locationRepository;
        private IAlbumRepository _albumRepository;
        private List<DetailViewModelBase> _openedDetailViewModels;

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

            _openedDetailViewModels = new List<DetailViewModelBase>();

            _eventAggregator.GetEvent<WriteAllMetadataEvent>()
                .Subscribe(WriteAllMetadata);
            _eventAggregator.GetEvent<DisplayMessageEvent>()
                .Subscribe(DisplayMessage);
            _eventAggregator.GetEvent<SaveAllDetailViewEvent>()
                .Subscribe(SaveAllDetailView);
            _eventAggregator.GetEvent<ErrorEvent>()
                .Subscribe(GetAnError);
        }

        public void RegisterOpenedDetailViews(DetailViewModelBase detailViewModel)
        {
            _openedDetailViewModels.Add(detailViewModel);            
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
