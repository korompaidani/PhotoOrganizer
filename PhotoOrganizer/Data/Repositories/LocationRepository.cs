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

        public async Task<string> TryGetCoordinatesByIdAsync(int locationId)
        {
            var result = await Context.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
            if(result != null) 
            {
                return result.Coordinates;
            }

            return null;
        }

        public async Task<bool> IsReferencedByPhotoAsync(int locationId)
        {
            return await Context.Photos.AsNoTracking().
                AnyAsync(p => p.LocationId == locationId);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
