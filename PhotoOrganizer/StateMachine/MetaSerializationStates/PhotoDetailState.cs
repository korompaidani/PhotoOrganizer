using PhotoOrganizer.FileHandler;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Helpers;
using PhotoOrganizer.UI.Services;
using Prism.Events;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public abstract class PhotoDetailState : IPhotoDetailState
    {
        protected IPhotoDetailContext _context;
        protected IBulkAttributeSetterService _bulkAttributeSetter;
        protected IPhotoMetaWrapperService _photoMetaWrapperService;
        protected IEventAggregator _eventAggregator;
        protected IMaintenanceRepository _maintenanceRepository;
        protected PhotoDetailInfo _photoDetailInfo;
        protected FileSystem _fileSystem;

        public IPhotoDetailState NextState { get; set; }

        public abstract void Handle();

        public void SetContextAndServices(
            IPhotoDetailContext context,
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService,
            IEventAggregator eventAggregator,
            IMaintenanceRepository maintenanceRepository,
            PhotoDetailInfo photoDetailInfo,
            FileSystem fileSystem)
        {
            _context = context;
            _bulkAttributeSetter = bulkAttributeSetter;
            _photoMetaWrapperService = photoMetaWrapperService;
            _eventAggregator = eventAggregator;
            _maintenanceRepository = maintenanceRepository;
            _photoDetailInfo = photoDetailInfo;
            _fileSystem = fileSystem;
        }
    }
}
