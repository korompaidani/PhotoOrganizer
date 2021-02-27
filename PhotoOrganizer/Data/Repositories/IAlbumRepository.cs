using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IAlbumRepository : IGenericRepository<Album>
    {
        Task<List<Photo>> GetAllPhotoAsync();
        Task ReloadPhotoAsync(int photoId);
    }
}