using PhotoOrganizer.FileHandler;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Helpers;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.StateMachine.MetaSerializationStates;
using Prism.Events;
using System;

namespace PhotoOrganizer.UI.StateMachine
{
    public class PhotoDetailContext : IPhotoDetailContext
    {
        private IPhotoDetailState _state;
        private IBulkAttributeSetterService _bulkAttributeSetter;
        private IPhotoMetaWrapperService _photoMetaWrapperService;
        private IEventAggregator _eventAggregator;
        private IMaintenanceRepository _maintenanceRepository;
        private FileSystem _fileSystem;
        private PhotoDetailInfo _photoDetailInfo;

        public PhotoDetailContext(
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService,
            IMaintenanceRepository maintenanceRepository,
            IEventAggregator eventAggregator,
            FileSystem fileSystem)
        {
            _bulkAttributeSetter = bulkAttributeSetter;
            _photoMetaWrapperService = photoMetaWrapperService;
            _eventAggregator = eventAggregator;
            _maintenanceRepository = maintenanceRepository;
            _fileSystem = fileSystem;
        }

        public void TransitionTo(IPhotoDetailState state, PhotoDetailInfo photoDetailInfo)
        {
            _state = state;
            if(photoDetailInfo != null)
            {
                _photoDetailInfo = photoDetailInfo;
            }
            state.SetContextAndServices(this, _bulkAttributeSetter, _photoMetaWrapperService, _eventAggregator, _maintenanceRepository, _photoDetailInfo, _fileSystem);            
        }

        public void RunWorkflow(IPhotoDetailState initialState, PhotoDetailInfo photoDetailInfo)
        {
            _photoDetailInfo = photoDetailInfo;

            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            _state = initialState;
            TransitionTo(initialState, _photoDetailInfo);
            _state.Handle();
        }
    }
}