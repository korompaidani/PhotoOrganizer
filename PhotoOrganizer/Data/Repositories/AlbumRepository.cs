using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.Generic;

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
    }
}
