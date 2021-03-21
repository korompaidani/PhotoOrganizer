using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IBackupService
    {
        Task CreateBackup(string path);
        void RestoreFromBackup(string path);
    }
}