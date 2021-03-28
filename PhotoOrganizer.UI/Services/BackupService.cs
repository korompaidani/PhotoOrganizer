using PhotoOrganizer.Common;
using PhotoOrganizer.DataAccess;
using PhotoOrganizer.FileHandler;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class BackupService : IBackupService
    {
        private string backupFolder;

        private BackupManager _backupManager;
        private PhotoOrganizerDbContext _photoOrganizerDbContext;
        private XmlWriterComponent _xmlWriter;

        public BackupService(BackupManager backupManager, PhotoOrganizerDbContext photoOrganizerDbContext, XmlWriterComponent xmlWriter)
        {
            _backupManager = backupManager;
            _photoOrganizerDbContext = photoOrganizerDbContext;
            _xmlWriter = xmlWriter;
        }

        public async Task CreateBackup(string path)
        {
            // TODO: save to config: use event
            if (path == null) { path = FilePaths.DefaultBackupFolder; }
            else { backupFolder = path; }
            
            try
            {
                // Use backupManager here
                await Task.Run(() => _backupManager.ReadAllTable(_photoOrganizerDbContext));
                // Write file here
                await _xmlWriter.WriteXmlAsync(path, _backupManager.AllTableData);
            }
            catch(Exception ex)
            {

            }
        }

        public void RestoreFromBackup(string path)
        {
            // TODO: load from config: use event
            if (path == null) { path = FilePaths.DefaultBackupFolder; }
            else { backupFolder = path; }
        }
    }
}