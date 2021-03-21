using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Resources.Language;
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

        public async Task<int> LoadSinglePhotoFromLibraryAsync()
        {
            var filePath = await _messageDialogService.SelectFileOrFolderDialogAsync(Environment.SpecialFolder.Personal.ToString(), TextResources.SelectPhoto_message, isFileDialog: true);

            var photo = _photoMetaWrapperService.CreatePhotoModelFromFile(filePath);

            var createdPhoto = CreateNewPhoto(photo);
            await _photoRepository.SaveAsync();
            return createdPhoto.Id;
        }

        public async Task LoadAllFromLibraryAsync()
        {
            if (await _photoRepository.HasPhotosAsync())
            {
                var answer = await _messageDialogService.ShowExtendOrOverwriteCancelDialogAsync(TextResources.ExtendOrOverwrite_message, TextResources.Question_windowTitle);
                if (answer == MessageDialogResult.Overwrite)
                {
                    answer = await _messageDialogService.ShowYesOrNoDialogAsync(TextResources.DoYouWantBackup_message, TextResources.Question_windowTitle);
                    if (answer == MessageDialogResult.Yes)
                    {
                        await CreateBackup();
                    }
                    if (answer == MessageDialogResult.Cancel)
                    {
                        return;
                    }
                    if(!await EraseFormerData())
                    {
                        return;
                    }
                }
                if(answer == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            string folderPath = await _messageDialogService.SelectFileOrFolderDialogAsync(Environment.SpecialFolder.Personal.ToString(), TextResources.SelectPhotoLocation_message);
            await _messageDialogService.ShowProgressDuringTaskAsync(TextResources.PleaseWait_windowTitle, TextResources.ReadingFiles_message, ReadAllFilesFromFolder, folderPath);
        }

        private async Task CreateBackup()
        {
            // save data here
            string backupFolder = await _messageDialogService.SelectFileOrFolderDialogAsync(Environment.SpecialFolder.Personal.ToString(), TextResources.ChooseFolder_windowTitle);
            if (string.IsNullOrEmpty(backupFolder))
            {
                return;
            }

            var entities = await _photoRepository.GetAllAsync();

            await _messageDialogService.ShowProgressDuringTaskAsync(TextResources.PleaseWait_windowTitle, TextResources.CreatingBackup_message, _backupService.CreateBackup, backupFolder);
        }

        private async Task<bool> EraseFormerData()
        {
            var result = await _messageDialogService.ShowYesOrNoDialogAsync(TextResources.ConfirmationBeforeErase_message, TextResources.Question_windowTitle);
            if (result == MessageDialogResult.No)
            {
                return false;
            }
            else
            {
                await _photoRepository.RemoveAllPhotoFromTableAsync();
                return true;
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