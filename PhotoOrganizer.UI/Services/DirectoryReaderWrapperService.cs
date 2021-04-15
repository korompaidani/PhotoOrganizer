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
using System.IO;
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
        private IThumbnailService _thumbnailService;
        private Dictionary<string, People> _peopleTempCache;
        private ApplicationContext _context;

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
            IPhotoMetaWrapperService photoMetaWrapperService,
            IThumbnailService thumbnailService)
        {
            PhotoRepository = photoRepository;
            _locationRepository = locationRepository;
            _directoryReader = directoryReader;
            _messageDialogService = messageDialogService;
            _backupService = backupService;
            _photoMetaWrapperService = photoMetaWrapperService;
            _thumbnailService = thumbnailService;
            _peopleTempCache = new Dictionary<string, People>();
            _context = Bootstrapper.Container.Resolve<ApplicationContext>();
        }

        public async Task<int> LoadSinglePhotoFromLibraryAsync()
        {
            var filePath = await _messageDialogService.SelectFileOrFolderDialogAsync(Environment.SpecialFolder.Personal.ToString(), TextResources.SelectPhoto_message, isFileDialog: true);

            var photo = _photoMetaWrapperService.CreatePhotoModelFromFile(filePath);
            await AddPeoplesToPhotoAsync(photo);
            CleanupCaches(_photoMetaWrapperService.PeopleNames);

            await _thumbnailService.CreateThumbnailAsync(Path.GetFullPath(filePath));
            var createdPhoto = CreateNewPhoto(photo);
            await PhotoRepository.SaveAsync();
            await _thumbnailService.PersistCacheAsync();
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

                    await _thumbnailService.TearDownThumbnailsAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    string innerException = string.Empty;
                    if (ex.InnerException != null && ex.InnerException.Message != null)
                    {
                        innerException = ex.InnerException.Message;
                    }
                    _context.AddErrorMessage(ErrorTypes.DataBaseError, ex.Message + innerException);
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

            var result = await ConvertFileNamesToPhotosAsync(folderPath);
            if (result)
            {
                await PhotoRepository.SaveAsync();
                await _thumbnailService.PersistCacheAsync();
                PhotoRepository.DisposeConnection();                
            }
        }

        private async Task<bool> ConvertFileNamesToPhotosAsync(string folderPath)
        {
            try
            {
                _directoryReader.ReadDirectory(folderPath);

                foreach (var file in _directoryReader.FileList)
                {
                    try
                    {
                        var photo = await Task<Photo>.Run(() => _photoMetaWrapperService.CreatePhotoModelFromFile(file.Key));                        
                        await _thumbnailService.CreateThumbnailAsync(Path.GetFullPath(file.Key));
                        CreateNewPhoto(photo);
                        await AddPeoplesToPhotoAsync(photo);
                        _photoMetaWrapperService.PeopleNames.Clear();
                    }
                    catch(Exception ex)
                    {
                        string innerException = string.Empty;
                        if (ex.InnerException != null && ex.InnerException.Message != null)
                        {
                            innerException = ex.InnerException.Message;
                        }
                        _context.AddErrorMessage(ErrorTypes.DirectoryReaderError, ex.Message + innerException);
                    }
                }

                _directoryReader.FileList.Clear();
                CleanupCaches(_photoMetaWrapperService.PeopleNames);

                return true;
            }
            catch (Exception ex)
            {                
                string innerException = string.Empty;
                if (ex.InnerException != null && ex.InnerException.Message != null)
                {
                    innerException = ex.InnerException.Message;
                }
                _context.AddErrorMessage(ErrorTypes.DirectoryReaderError, ex.Message + innerException);
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

        private async Task AddPeoplesToPhotoAsync(Photo photo)
        {
            var peopleNames = _photoMetaWrapperService.PeopleNames;
            if (peopleNames != null && peopleNames.Count != 0)
            {
                foreach (var peopleName in peopleNames)
                {
                    var result = await PhotoRepository.TryGetAnyPeopleByDisplayName(peopleName);
                    if (result == null)
                    {
                        People newPeople = null;

                        if (_peopleTempCache.TryGetValue(peopleName, out newPeople))
                        {
                            result = newPeople;
                        }
                        else
                        {
                            newPeople = new People { DisplayName = peopleName };
                            _peopleTempCache.Add(peopleName, newPeople);
                            result = newPeople;
                        }
                    }

                    photo.Peoples.Add(result);
                }
            }
        }

        private void CleanupCaches(HashSet<string> peopleGuestCache)
        {
            _peopleTempCache.Clear();
            peopleGuestCache.Clear();
        }
    }
}