using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.StateMachine.MetaSerializationStates;
using Prism.Events;
using System;
using System.IO;

namespace PhotoOrganizer.UI.StateMachine
{
    public class PhotoDetailContext : IPhotoDetailContext
    {
        private IPhotoDetailState _state;
        private IBulkAttributeSetterService _bulkAttributeSetter;
        private IPhotoMetaWrapperService _photoMetaWrapperService;
        private IEventAggregator _eventAggregator;
        private string _photoOriginalPath;

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
            _state.SetContextAndServices(this, _bulkAttributeSetter, _photoMetaWrapperService, _eventAggregator, _photoOriginalPath);
        }

        public void RunWorkflow(IPhotoDetailState initialState, string photoOriginalPath)
        {
            _photoOriginalPath = Path.GetFullPath(photoOriginalPath);

            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            TransitionTo(initialState);

            _state.Handle();
        }
    }
}