using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class PeopleRepository : GenericRepository<People, PhotoOrganizerDbContext>, IPeopleRepository
    {
        public PeopleRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }
    }
}