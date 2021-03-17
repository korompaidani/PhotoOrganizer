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

        public abstract void Handle();

        public void SetContextAndServices(
            IPhotoDetailContext context,
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService,
            IEventAggregator eventAggregator)
        {
            _context = context;
            _bulkAttributeSetter = bulkAttributeSetter;
            _photoMetaWrapperService = photoMetaWrapperService;
            _eventAggregator = eventAggregator;
        }
    }
}
