using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.View.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class DirectoryReaderWrapperService : IDirectoryReaderWrapperService
    {
        private IPhotoRepository _photoRepository;
        private DirectoryReader _directoryReader;
        private IMessageDialogService _messageDialogService;
        private IBackupService _backupService;

        public DirectoryReaderWrapperService(
            DirectoryReader directoryReader, 
            IPhotoRepository photoRepository,
            IMessageDialogService messageDialogService,
            IBackupService backupService)
        {
            _photoRepository = photoRepository;
            _directoryReader = directoryReader;
            _messageDialogService = messageDialogService;
            _backupService = backupService;
        }

        public async Task LoadAllFromLibraryAsync()
        {
            if (await _photoRepository.HasPhotosAsync())
            {
                var answer = await _messageDialogService.ShowOkCancelDialogAsync("The database has entry(s). Would you like to save data first before erase all photo data?", "Question");
                if (answer == MessageDialogResult.Ok)
                {
                    // save data here
                    var entities = await _photoRepository.GetAllAsync();
                    _backupService.CreateBackup(null);
                }
                else
                {
                    return;
                }

                var result = await _messageDialogService.ShowOkCancelDialogAsync("This operation will erase all previous data from Database. Are you sure to load new library data?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    await _photoRepository.RemoveAllPhotoFromTableAsync();
                }
            }

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