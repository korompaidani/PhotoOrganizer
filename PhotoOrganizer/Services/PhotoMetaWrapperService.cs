using PhotoOrganizer.FileHandler;
using PhotoOrganizer.UI.Data.Repositories;

namespace PhotoOrganizer.UI.Services
{
    public class PhotoMetaWrapperService : IPhotoMetaWrapperService
    {
        private IPhotoRepository _photoRepository;
        private ExifToFileWriter _exifToFileWriter;

        public PhotoMetaWrapperService(IPhotoRepository photoRepository, ExifToFileWriter exifToFileWriter)
        {
            _photoRepository = photoRepository;
            _exifToFileWriter = exifToFileWriter;
        }
    }
}
