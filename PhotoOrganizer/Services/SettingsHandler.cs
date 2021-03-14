using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class SettingsHandler : ISettingsHandler
    {
        private IPageSizeService _pageSizeService;
        private JsonFileHandler<Settings> _jsonFileHandler;

        public SettingsHandler(IPageSizeService pageSizeService)
        {
            _jsonFileHandler = new JsonFileHandler<Settings>();
            _pageSizeService = pageSizeService;
        }

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
                return await _jsonFileHandler.ReadModelFromFileAsync();
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
    }
}
