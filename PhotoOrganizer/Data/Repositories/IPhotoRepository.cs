using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IPhotoRepository
    {
        Task<Photo> GetByIdAsync(int photoId);
        Task SaveAsync();
        bool HasChanges();
        Task<bool> HasPhotos();
        void Add(Photo photo);
        void Remove(Photo model);
        Task TruncatePhotoTable();
    }
}
