using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.View.Services;
using System;
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
        private IPhotoMetaWrapperService _photoMetaWrapperService;

        public DirectoryReaderWrapperService(
            DirectoryReader directoryReader, 
            IPhotoRepository photoRepository,
            IMessageDialogService messageDialogService,
            IBackupService backupService,
            IPhotoMetaWrapperService photoMetaWrapperService)
        {
            _photoRepository = photoRepository;
            _directoryReader = directoryReader;
            _messageDialogService = messageDialogService;
            _backupService = backupService;
            _photoMetaWrapperService = photoMetaWrapperService;
        }

        public async Task LoadAllFromLibraryAsync()
        {
            if (await _photoRepository.HasPhotosAsync())
            {
                var answer = await _messageDialogService.ShowExtendOrOverwriteCancelDialogAsync("The database has entry(s). Would you like to 'Extend' existing or 'Overwrite'?", "Question");
                if (answer == MessageDialogResult.Overwrite)
                {
                    answer = await _messageDialogService.ShowYesOrNoDialogAsync("Would you like to backup database first before erase all photo data?", "Question");
                    if (answer == MessageDialogResult.Yes)
                    {
                        await CreateBackup();
                        await EraseFormerData();
                    }
                    if (answer == MessageDialogResult.Cancel)
                    {
                        return;
                    }
                }
                if(answer == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            string folderPath = await _messageDialogService.SelectFolderPathAsync(Environment.SpecialFolder.Personal.ToString(), "Please select a 'photo' location:");
            await _messageDialogService.ShowProgressDuringTaskAsync("Please wait", "Reading files...", ReadAllFilesFromFolder, folderPath);
        }

        private async Task CreateBackup()
        {
            // save data here
            string backupFolder = await _messageDialogService.SelectFolderPathAsync(Environment.SpecialFolder.Personal.ToString(), "Please choose folder for 'backup':");
            if (string.IsNullOrEmpty(backupFolder))
            {
                return;
            }

            var entities = await _photoRepository.GetAllAsync();

            await _messageDialogService.ShowProgressDuringTaskAsync("Please wait", "Creating backup...", _backupService.CreateBackup, backupFolder);
        }

        private async Task EraseFormerData()
        {
            var result = await _messageDialogService.ShowYesOrNoDialogAsync("This operation will erase all previous data from Database. Are you sure to load new library data?", "Question");
            if (result == MessageDialogResult.No)
            {
                return;
            }
            else
            {
                await _photoRepository.RemoveAllPhotoFromTableAsync();
            }
        }

        private async Task ReadAllFilesFromFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            var result = await Task<bool>.Run(() => ConvertFileNamesToPhotos(folderPath));
            if (result)
            {
                await _photoRepository.SaveAsync();
            }
        }

        private bool ConvertFileNamesToPhotos(string folderPath)
        {
            try
            {
                _directoryReader.ReadDirectory(folderPath);

                var list = new List<Photo>();
                foreach (var file in _directoryReader.FileList)
                {
                    var photo = _photoMetaWrapperService.CreatePhotoModelFromFile(file.Key);
                    list.Add(photo);
                }

                foreach (var photo in list)
                {
                    CreateNewPhoto(photo);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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