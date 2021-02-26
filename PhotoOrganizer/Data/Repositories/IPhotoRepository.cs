using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{

    public interface IPhotoRepository : IGenericRepository<Photo>
    {
        Task<bool> HasAlbums(int photoId);
        void RemovePeople(People model);
        Task<bool> HasPhotosAsync();
        Task RemoveAllPhotoFromTableAsync();
        Task<int?> GetMaxPhotoIdAsync();
        void AddRange(Photo[] photos);
        Task AddRangeAsync(Photo[] photos);
    }
}
