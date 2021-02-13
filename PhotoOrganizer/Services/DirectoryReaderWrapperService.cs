using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class DirectoryReaderWrapperService : IDirectoryReaderWrapperService
    {
        private IPhotoRepository _photoRepository;
        private DirectoryReader _directoryReader;

        public DirectoryReaderWrapperService(DirectoryReader directoryReader, IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
            _directoryReader = directoryReader;
        }

        public async Task LoadAllFromLibraryAsync()
        {
            foreach (var photo in ConvertFileNamesToPhotos())
            {
                CreateNewPhoto(photo);
            }
            await _photoRepository.SaveAsync();
        }

        private Photo[] ConvertFileNamesToPhotos()
        {
            var list = new List<Photo>();
            foreach (var file in _directoryReader.FileList)
            {
                list.Add(new Photo { FullPath = file.Key, Title = file.Value });
            }

            return list.ToArray();
        }

        private Photo CreateNewPhoto(Photo photo = null)
        {
            if (photo == null)
            {
                photo = new Photo();
            }

            _photoRepository.Add(photo);
            return photo;
        }
    }
}