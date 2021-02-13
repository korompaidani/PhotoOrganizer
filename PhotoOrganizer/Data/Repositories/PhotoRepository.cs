using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System;
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

        public async Task<bool> HasPhotosAsync()
        {
            return await Context.Photos.CountAsync() != 0 ? true : false;
        }

        public void AddRange(Photo[] photos)
        {
            Context.Photos.AddRange(photos);
        }

        public async Task AddRangeAsync(Photo[] photos)
        {
            // Close context after 100
            // context.Configuration.AutoDetectChangesEnabled = false;
            const int bufferSize = 100;
            int bufferCounter = 0;

            foreach (var photo in photos)
            {
                if (bufferCounter++ == bufferSize)
                {

                }

                Context.Photos.Add(photo);
            }

            await SaveAsync();
        }

        public async Task<int?> GetMaxPhotoIdAsync()
        {
            return await Context.Photos.MaxAsync(p => (int?)p.Id);
        }

        // Don't forget truncate will drop indexing and tracking as well
        public async Task TruncatePhotoTable()
        {
            var photos = await GetAllAsync();
            foreach (var photo in photos)
            {
                Context.Photos.Remove(photo);
            }

            await SaveAsync();
        }
    }
}
