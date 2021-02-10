using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class AlbumRepository : GenericRepository<Album, PhotoOrganizerDbContext>, IAlbumRepository
    {
        public AlbumRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public async override Task<Album> GetByIdAsync(int id)
        {
            return await Context.Albums
                .Include(a => a.Photos)
                .SingleAsync(a => a.Id == id);
        }

        public async Task<List<Photo>> GetAllFriendAsync()
        {
            return await Context.Set<Photo>()
                .ToListAsync();
        }

        public async Task ReloadPhotoAsync(int photoId)
        {
            var dbEntitiyEntry = Context.ChangeTracker.Entries<Photo>()
                .SingleOrDefault(db => db.Entity.Id == photoId);
            if(dbEntitiyEntry != null)
            {
                await dbEntitiyEntry.ReloadAsync();
            }
        }
    }
}
