using PhotoOrganizer.FileHandler;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Helpers;
using PhotoOrganizer.UI.Services;
using Prism.Events;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public interface IPhotoDetailState
    {
        IPhotoDetailState NextState { get; set; }

        void SetContextAndServices(
            IPhotoDetailContext context,
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService,
            IEventAggregator eventAggregator,
            IMaintenanceRepository maintenanceRepository,
            PhotoDetailInfo photoDetailInfo,
            FileSystem fileSystem);
        void Handle();
    }
}
