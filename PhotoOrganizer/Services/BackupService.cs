using PhotoOrganizer.Common;
using PhotoOrganizer.DataAccess;
using PhotoOrganizer.FileHandler;

namespace PhotoOrganizer.UI.Services
{
    public class BackupService : IBackupService
    {
        private string backupFolder;

        private BackupManager _backupManager;
        private PhotoOrganizerDbContext _photoOrganizerDbContext;

        public BackupService(BackupManager backupManager, PhotoOrganizerDbContext photoOrganizerDbContext)
        {
            _backupManager = backupManager;
            _photoOrganizerDbContext = photoOrganizerDbContext;
        }

        public void CreateBackup(string path)
        {
            // TODO: save to config: use event
            if (path == null) { path = FilePaths.DefaultBackupFolder; }
            else { backupFolder = path; }

            // Use backupManager here
            _backupManager.ReadAllTable(_photoOrganizerDbContext);
            // Write file here
        }

        public void RestoreFromBackup(string path)
        {
            // TODO: load from config: use event
            if (path == null) { path = FilePaths.DefaultBackupFolder; }
            else { backupFolder = path; }
        }
    }
}