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

        public async Task<List<Photo>> GetAllAsync()
        {
            using(var context = _contextCreator())
            {
                return await context.Photos.AsNoTracking().ToListAsync();
            }
        }
    }
}
