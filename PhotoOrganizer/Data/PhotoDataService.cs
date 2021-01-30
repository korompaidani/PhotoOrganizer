using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    public class PhotoDataService : IPhotoDataService
    {
        private Func<PhotoOrganizerDbContext> _contextCreator;

        public PhotoDataService(Func<PhotoOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<Photo> GetByIdAsync(int photoId)
        {
            using(var context = _contextCreator())
            {
                return await context.Photos.AsNoTracking().SingleAsync(p => p.Id == photoId);
            }
        }

        public async Task SaveAsync(Photo photo)
        {
            using (var context = _contextCreator())
            {
                context.Photos.Attach(photo);
                context.Entry(photo).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
