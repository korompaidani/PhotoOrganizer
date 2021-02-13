using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IDirectoryReaderWrapperService
    {
        Task LoadAllFromLibraryAsync();
    }
}