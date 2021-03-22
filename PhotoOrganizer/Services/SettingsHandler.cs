using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class SettingsHandler : ISettingsHandler
    {
        private IPageSizeService _pageSizeService;
        private JsonFileHandler<Settings> _jsonFileHandler;
        private Settings _initialSettings;

        public SettingsHandler(IPageSizeService pageSizeService)
        {
            _jsonFileHandler = new JsonFileHandler<Settings>();
            _pageSizeService = pageSizeService;
        }

        // TODO: each part should be register for an event provided by this handler
        public async Task ApplySettingsAsync(Settings settings)
        {
            if(settings != null)
            {
                await _pageSizeService.SetPageSize(settings.PageSize);
            }
        }

        public async Task<Settings> LoadSettingsAsync()
        {
            try
            {
                if(_initialSettings == null)
                {
                    return await _jsonFileHandler.ReadModelFromFileAsync();
                }
                return _initialSettings;
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveSettingsAsync(Settings settings)
        {
            await _jsonFileHandler.WriteModelToFileAsync(settings);
        }

        public string GetLanguageSettings()
        {
            if (_initialSettings == null)
            {
                try
                {
                    _initialSettings = _jsonFileHandler.InitialReadModelFromFile();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return _initialSettings.Language;
        }
    }
}
