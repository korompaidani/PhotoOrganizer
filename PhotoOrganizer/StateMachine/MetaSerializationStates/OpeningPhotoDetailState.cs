using Autofac;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Startup;

namespace PhotoOrganizer.UI.StateMachine.MetaSerializationStates
{
    public class OpeningPhotoDetailState : PhotoDetailState
    {
        public override async void Handle()
        {
            // File operations
            string tempFileFullPath;
            _fileSystem.CreateTemp(_photoDetailInfo.FullFilePath, out tempFileFullPath);

            // Write file name to the db
            if(tempFileFullPath != null)
            {
                _photoDetailInfo.FileEntry = new FileEntry { ImageFilePath = tempFileFullPath };
                _photoDetailInfo.FullTempFilePath = tempFileFullPath;
                _maintenanceRepository.Add(_photoDetailInfo.FileEntry);
                await _maintenanceRepository.SaveAsync();
            }
            else
            {
                // Message later about error
                return;
            }            

            var newState = Bootstrapper.Container.Resolve<OpenPhotoDetailState>();
            _context.TransitionTo(newState, _photoDetailInfo);
            newState.Handle();
        }
    }
}
