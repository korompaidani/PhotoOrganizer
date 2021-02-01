using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private PhotoOrganizerDbContext _context;

        public PhotoRepository(PhotoOrganizerDbContext context)
        {
            _context = context;
        }

        public void Add(Photo photo)
        {
            _context.Photos.Add(photo);
        }

        public void AddRange(Photo[] photos)
        {
            _context.Photos.AddRange(photos);            
        }

        private PhotoOrganizerDbContext AddToContext(PhotoOrganizerDbContext context,
        Photo entity, int count, int commitCount, bool recreateContext)
        {
            context.Photos.Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new PhotoOrganizerDbContext();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        public async Task AddRangeAsync(Photo[] photos)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                PhotoOrganizerDbContext context = null;
                try
                {
                    context = new PhotoOrganizerDbContext();
                    context.Configuration.AutoDetectChangesEnabled = false;

                    int count = 0;
                    foreach (var entityToInsert in photos)
                    {
                        ++count;
                        context = AddToContext(context, entityToInsert, count, 100, true);
                    }

                    await context.SaveChangesAsync();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                scope.Complete();
            }

            // Close context after 100
            // context.Configuration.AutoDetectChangesEnabled = false;
            //const int bufferSize = 100;
            //int bufferCounter = 0;

            //foreach(var photo in photos)
            //{
            //    if(bufferCounter++ == bufferSize)
            //    {

            //    }
                
            //    _context.Photos.Add(photo);
            //}

            //await SaveAsync();
        }

        public async Task<int?> GetMaxPhotoIdAsync()
        {
            return await _context.Photos.MaxAsync(p => (int?)p.Id);
        }

        public async Task<List<Photo>> GetAllPhotosAsync()
        {
            return await _context.Photos.ToListAsync();
        }

        public async Task<Photo> GetByIdAsync(int photoId)
        {            
            return await _context.Photos.SingleAsync(p => p.Id == photoId);
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public async Task<bool> HasPhotosAsync()
        {
            return await _context.Photos.CountAsync() != 0 ? true : false;
        }

        public void Remove(Photo model)
        {
            _context.Photos.Remove(model);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task TruncatePhotoTable()
        {
            var photos = await GetAllPhotosAsync();
            foreach (var photo in photos)
            {
                _context.Photos.Remove(photo);
            }

            await SaveAsync();
        }
    }
}
