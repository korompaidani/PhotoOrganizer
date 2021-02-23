using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class LocationRepository : GenericRepository<Location, PhotoOrganizerDbContext>, ILocationRepository
    {
        public LocationRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public async Task<bool> IsReferencedByPhotoAsync(int locationId)
        {
            return await Context.Photos.AsNoTracking().
                AnyAsync(p => p.LocationId == locationId);
        }
    }
}
