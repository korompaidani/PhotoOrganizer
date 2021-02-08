using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class PhotoRepository : GenericRepository<Photo, PhotoOrganizerDbContext>, IPhotoRepository
    {
        public PhotoRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public override async Task<Photo> GetByIdAsync(int photoId)
        {
            return await Context.Photos
                .Include(p => p.Peoples)
                .SingleAsync(p => p.Id == photoId);
        }

        public async Task<bool> HasAlbums(int photoId)
        {
            return await Context.Albums.AsNoTracking()
                .Include(a => a.Photos)
                .AnyAsync(a => a.Photos.Any(p => p.Id == photoId));
        }

        public void RemovePeople(People model)
        {
            Context.People.Remove(model);
        }
    }
}
