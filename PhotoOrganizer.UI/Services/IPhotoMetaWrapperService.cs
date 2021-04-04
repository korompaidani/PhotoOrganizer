using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IPhotoMetaWrapperService
    {
        HashSet<string> PeopleNames { get; }
        bool WriteMetaInfoToSingleFile(Photo photoModel, string targetFile);
        Task WriteMetaInfoToAllFileAsync();
        Photo CreatePhotoModelFromFile(string filePath);
    }
}