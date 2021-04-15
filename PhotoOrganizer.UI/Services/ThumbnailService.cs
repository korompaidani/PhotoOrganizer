using Autofac;
using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private IThumbnailCreator _thumbnailCreator;
        private IMaintenanceRepository _maintenanceRepository;
        private ApplicationContext _context;
        private Dictionary<string, string> _thumbnailCache;

        public ThumbnailService(
            IThumbnailCreator thumbnailCreator,
            IMaintenanceRepository maintenanceRepository)
        {
            _thumbnailCreator = thumbnailCreator;
            _maintenanceRepository = maintenanceRepository;
            _context = Bootstrapper.Container.Resolve<ApplicationContext>();
            _thumbnailCache = new Dictionary<string, string>();
        }

        public async Task CreateThumbnailAsync(string imagePath)
        {
            try
            {
                var thumbnailPath = await Task.Run(() => _thumbnailCreator.WriteThumbnailWithPath(imagePath));
                _thumbnailCache.Add(imagePath, thumbnailPath);

                var fileEntry = new FileEntry { OriginalImagePath = imagePath, ThumbnailPath = thumbnailPath };
                _maintenanceRepository.Add(fileEntry);
            }
            catch (Exception ex)
            {
                _context.AddErrorMessage(ErrorTypes.ThumbnailError, ex.Message);
            }
        }

        public string GetThumbnailPath(string imagePath)
        {
            string thumbnailPath = null;
            _thumbnailCache.TryGetValue(imagePath, out thumbnailPath);

            if (string.IsNullOrEmpty(thumbnailPath))
            {
                _context.AddErrorMessage(ErrorTypes.ThumbnailError, imagePath);
            }

            return thumbnailPath;
        }

        public async Task LoadCacheAsync()
        {
            var entries = await _maintenanceRepository.GetAllNonFlaggedAsync();
            foreach(var entry in entries)
            {
                _thumbnailCache[entry.OriginalImagePath] = entry.ThumbnailPath;
            }
        }

        public async Task PersistCacheAsync()
        {
            await _maintenanceRepository.SaveAsync();
        }

        public async Task ClearRemainedThumbnailsAsync()
        {
            try
            {
                var flaggedEntries = await _maintenanceRepository.GetAllFlaggedAsync();

                if(flaggedEntries == null || flaggedEntries.Count == 0)
                {
                    return;
                }

                foreach (var entry in flaggedEntries)
                {
                    _thumbnailCreator.DeleteThumbnail(entry.ThumbnailPath);
                }

                await _maintenanceRepository.RemoveAllFlaggedEntriesFromTableAsync();
            }
            catch (UnauthorizedAccessException ex)
            {
                _context.AddErrorMessage(ErrorTypes.ThumbnailError, ex.Message);
            }
        }

        public async Task TearDownThumbnailsAsync()
        {
            foreach (var file in _thumbnailCache)
            {
                var entry = await _maintenanceRepository.GetEntrybyOriginalPath(file.Key);
                if(entry != null)
                {
                    entry.OriginalImagePath = "x";
                }
            }
            await _maintenanceRepository.SaveAsync();

            _thumbnailCache.Clear();
        }
    }
}
