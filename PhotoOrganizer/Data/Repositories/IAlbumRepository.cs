using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IAlbumRepository : IGenericRepository<Album>
    {
        Task<Album> GetByIdAsync(int id);
    }
}