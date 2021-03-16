using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IPhotoMetaWrapperService
    {
        bool WriteMetaInfoToSingleFile(Photo photoModel);
        Task WriteMetaInfoToAllFileAsync();
        Photo CreatePhotoFromMeta(string filePath);
    }
}