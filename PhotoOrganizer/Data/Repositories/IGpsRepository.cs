using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IGpsRepository : IGenericRepository<Gps>
    {
        Task<bool> IsReferencedByPhotoAsync(int gpsId);
    }
}