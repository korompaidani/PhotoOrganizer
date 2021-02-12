using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class YearRepository : GenericRepository<Year, PhotoOrganizerDbContext>, IYearRepository
    {
        public YearRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public async Task<bool> IsReferencedByPhotoAsync(int yearId)
        {
            return await Context.Photos.AsNoTracking().
                AnyAsync(p => p.YearId == yearId);
        }
    }
}
