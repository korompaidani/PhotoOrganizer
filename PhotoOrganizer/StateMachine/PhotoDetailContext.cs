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

        public PhotoDetailContext(
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService,
            IEventAggregator eventAggregator)
        {
            _bulkAttributeSetter = bulkAttributeSetter;
            _photoMetaWrapperService = photoMetaWrapperService;
            _eventAggregator = eventAggregator;
        }

        public void TransitionTo(IPhotoDetailState state)
        {
            _state = state;
            _state.SetContextAndServices(this, _bulkAttributeSetter, _photoMetaWrapperService, _eventAggregator);
        }

        public void RunWorkflow(IPhotoDetailState initialState)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            TransitionTo(initialState);

            _state.Handle();
        }
    }
}