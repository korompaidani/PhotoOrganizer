using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IPhotoRepository
    {
        Task<Photo> GetByIdAsync(int photoId);
        Task SaveAsync();
        bool HasChanges();
        void Add(Photo photo);
    }
}
