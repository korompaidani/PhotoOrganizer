using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IThumbnailService
    {
        Task LoadCacheAsync();
        Task PersistCacheAsync();
        Task TearDownThumbnailsAsync();
        Task CreateThumbnailAsync(string imagePath);
        Task ClearRemainedThumbnailsAsync();
        string GetThumbnailPath(string imagePath);
    }
}
