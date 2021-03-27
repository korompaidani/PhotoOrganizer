using Autofac;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Startup;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public class OpenPhotoDetailState : PhotoDetailState
    {
        public override void Handle()
        {
            _eventAggregator.GetEvent<WriteMetadataEvent>()
                .Subscribe(OnWriteMetadata);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Subscribe(AfterDetailClosed);
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args)
        {
            if (args.Id == _photoDetailInfo.Id)
            {
                _eventAggregator.GetEvent<AfterDetailClosedEvent>().Unsubscribe(AfterDetailClosed);
                _eventAggregator.GetEvent<WriteMetadataEvent>().Unsubscribe(OnWriteMetadata);

                var newState = Bootstrapper.Container.Resolve<ClosingPhotoDetailState>();
                _context.TransitionTo(newState, _photoDetailInfo);
                newState.Handle();
            }            
        }

        private void OnWriteMetadata(WriteMetadataEventArgs args)
        {
            if(args.PhotoId == _photoDetailInfo.Id)
            {
                var result = _photoMetaWrapperService.WriteMetaInfoToSingleFile(args.Photo, _photoDetailInfo.FullTempFilePath);   

                _eventAggregator.GetEvent<WriteMetadataFinishedEvent>().Publish(
                    new WriteMetadataFinishedEventArgs
                    {
                        PhotoId = _photoDetailInfo.Id,
                        IsSuccesfullyDone = result
                    });
            }
        }
    }
}
