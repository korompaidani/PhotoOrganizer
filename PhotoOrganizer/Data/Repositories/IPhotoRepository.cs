using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IPhotoRepository
    {
        Task<Photo> GetByIdAsync(int photoId);
        Task SaveAsync();
        bool HasChanges();
        Task<bool> HasPhotosAsync();
        Task<int?> GetMaxPhotoIdAsync();
        Task<List<Photo>> GetAllPhotosAsync();
        void Add(Photo photo);
        void AddRange(Photo[] photos);
        Task AddRangeAsync(Photo[] photos);
        void Remove(Photo model);
        Task DeleteAllEntriesFromTableAsync();
    }
}
