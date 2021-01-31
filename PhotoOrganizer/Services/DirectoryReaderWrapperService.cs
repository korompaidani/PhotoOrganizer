using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using System.Collections.Generic;

namespace PhotoOrganizer.UI.Services
{
    public class DirectoryReaderWrapperService : IDirectoryReaderWrapperService
    {
        private DirectoryReader _directoryReader;

        public DirectoryReaderWrapperService(DirectoryReader directoryReader)
        {
            _directoryReader = directoryReader;
        }

        public Photo[] ConvertFileNamesToPhotos()
        {
            var list = new List<Photo>();
            foreach (var file in _directoryReader.FileList)
            {
                list.Add(new Photo { FullPath = file.Key, Title = file.Value });
            }

            return list.ToArray();
        }
    }
}
