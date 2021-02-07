using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Data.Entity;
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

        public void RemovePeople(People model)
        {
            Context.People.Remove(model);
        }
    }
}
