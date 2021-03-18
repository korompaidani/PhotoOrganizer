using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IPhotoMetaWrapperService
    {
        bool WriteMetaInfoToSingleFile(Photo photoModel, string targetFile);
        Task WriteMetaInfoToAllFileAsync();
        Photo CreatePhotoModelFromFile(string filePath);
    }
}