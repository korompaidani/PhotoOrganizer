using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

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

        public async Task<Photo> GetByIdAsync(int photoId)
        {
            return await _context.Photos.SingleAsync(p => p.Id == photoId);
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
