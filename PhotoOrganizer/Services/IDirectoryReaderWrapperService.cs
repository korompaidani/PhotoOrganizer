using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Services
{
    public interface IDirectoryReaderWrapperService
    {
        Photo[] ConvertFileNamesToPhotos();
    }
}