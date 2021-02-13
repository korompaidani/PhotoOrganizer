namespace PhotoOrganizer.UI.Services
{
    public interface IBackupService
    {
        void CreateBackup(string path);
        void RestoreFromBackup(string path);
    }
}