using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IDirectoryReaderWrapperService
    {
        Task<int> LoadSinglePhotoFromLibraryAsync();
        Task LoadAllFromLibraryAsync();
        Task<bool> EraseFormerData();
        Task CreateBackup();
    }
}