using Autofac;
using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Resources.Language;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.View.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class DirectoryReaderWrapperService : IDirectoryReaderWrapperService
    {
        private IPhotoRepository _photoRepository;
        private ILocationRepository _locationRepository;
        private DirectoryReader _directoryReader;
        private IMessageDialogService _messageDialogService;
        private IBackupService _backupService;
        private IPhotoMetaWrapperService _photoMetaWrapperService;

        public IPhotoRepository PhotoRepository
        {
            get 
            {
                if(_photoRepository == null || _photoRepository.IsDisposed)
                {
                    _photoRepository = Bootstrapper.Container.Resolve<IPhotoRepository>();
                }
                return _photoRepository;
            }
            set
            {
                _photoRepository = value;
            }
        }

        public ILocationRepository LocationRepository
        {
            get
            {
                if (_locationRepository == null || _locationRepository.IsDisposed)
                {
                    _locationRepository = Bootstrapper.Container.Resolve<ILocationRepository>();
                }
                return _locationRepository;
            }
            set
            {
                _locationRepository = value;
            }
        }

        public DirectoryReaderWrapperService(
            DirectoryReader directoryReader,
            IPhotoRepository photoRepository,
            ILocationRepository locationRepository,
            IMessageDialogService messageDialogService,
            IBackupService backupService,
            IPhotoMetaWrapperService photoMetaWrapperService)
        {
            PhotoRepository = photoRepository;
            _locationRepository = locationRepository;
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
            await PhotoRepository.SaveAsync();
            PhotoRepository.DisposeConnection();
            return createdPhoto.Id;
        }

        public async Task LoadAllFromLibraryAsync()
        {
            if (await PhotoRepository.HasPhotosAsync())
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
                    if (!await EraseFormerData())
                    {
                        return;
                    }
                }
                if (answer == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            string folderPath = await _messageDialogService.SelectFileOrFolderDialogAsync(Environment.SpecialFolder.Personal.ToString(), TextResources.SelectPhotoLocation_message);
            await _messageDialogService.ShowProgressDuringTaskAsync(TextResources.PleaseWait_windowTitle, TextResources.ReadingFiles_message, ReadAllFilesFromFolder, folderPath);
        }

        public async Task<bool> EraseFormerData()
        {
            var result = await _messageDialogService.ShowYesOrNoDialogAsync(TextResources.ConfirmationBeforeErase_message, TextResources.Question_windowTitle);
            if (result == MessageDialogResult.Yes)
            {
                try
                {
                    await PhotoRepository.RemoveAllPhotoFromTableAsync();
                    PhotoRepository.DisposeConnection();

                    await LocationRepository.RemoveAllPhotoFromTableAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    var context = Bootstrapper.Container.Resolve<ApplicationContext>();
                    string innerException = string.Empty;
                    if (ex.InnerException != null && ex.InnerException.Message != null)
                    {
                        innerException = ex.InnerException.Message;
                    }
                    context.AddErrorMessage(ErrorTypes.DataBaseError, ex.Message + innerException);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task CreateBackup()
        {
            // save data here
            string backupFolder = await _messageDialogService.SelectFileOrFolderDialogAsync(Environment.SpecialFolder.Personal.ToString(), TextResources.ChooseFolder_windowTitle);
            if (string.IsNullOrEmpty(backupFolder))
            {
                return;
            }

            var entities = await PhotoRepository.GetAllAsync();

            await _messageDialogService.ShowProgressDuringTaskAsync(TextResources.PleaseWait_windowTitle, TextResources.CreatingBackup_message, _backupService.CreateBackup, backupFolder);
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
                await PhotoRepository.SaveAsync();
                PhotoRepository.DisposeConnection();
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
                    try
                    {
                        var photo = _photoMetaWrapperService.CreatePhotoModelFromFile(file.Key);
                        list.Add(photo);
                    }
                    catch
                    {
                    }
                }

                foreach (var photo in list)
                {
                    CreateNewPhoto(photo);
                }

                return true;
            }
            catch (Exception ex)
            {
                var context = Bootstrapper.Container.Resolve<ApplicationContext>();
                string innerException = string.Empty;
                if (ex.InnerException != null && ex.InnerException.Message != null)
                {
                    innerException = ex.InnerException.Message;
                }
                context.AddErrorMessage(ErrorTypes.DirectoryReaderError, ex.Message + innerException);
                return false;
            }
        }

        private Photo CreateNewPhoto(Photo photo = null)
        {
            if (photo == null)
            {
                photo = new Photo();
            }

            PhotoRepository.Add(photo);
            return photo;
        }
    }
}