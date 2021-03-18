using Autofac;
using PhotoOrganizer.UI.Startup;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public class ClosingPhotoDetailState : PhotoDetailState
    {
        public override async void Handle()
        {
            var result = _fileSystem.OverWriteOriginalByTemp(_photoDetailInfo.FullFilePath, _photoDetailInfo.FullTempFilePath);

            var entry = await _maintenanceRepository.GetByIdAsync(_photoDetailInfo.FileEntry.Id);
            if (result && entry != null)
            {
                _maintenanceRepository.Remove(entry);
                await _maintenanceRepository.SaveAsync();
            }

            var newState = Bootstrapper.Container.Resolve<ClosedPhotoDetailState>();
            _context.TransitionTo(newState, _photoDetailInfo);
            newState.Handle();
        }
    }
}
