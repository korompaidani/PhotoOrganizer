using PhotoOrganizer.UI.Services;
using Prism.Events;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public interface IPhotoDetailState
    {
        void SetContextAndServices(IPhotoDetailContext context, IBulkAttributeSetterService bulkAttributeSetter, IPhotoMetaWrapperService photoMetaWrapperService, IEventAggregator eventAggregator);
        void Handle();
    }
}
