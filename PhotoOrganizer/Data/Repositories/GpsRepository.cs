using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class GpsRepository : GenericRepository<Gps, PhotoOrganizerDbContext>, IGpsRepository
    {
        public GpsRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public async Task<bool> IsReferencedByPhotoAsync(int gpsId)
        {
            return await Context.Photos.AsNoTracking().
                AnyAsync(p => p.LocationId == gpsId);
        }
    }
}
