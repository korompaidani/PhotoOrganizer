using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class MaintenanceRepository : GenericRepository<FileEntry, PhotoOrganizerDbContext>, IMaintenanceRepository
    {
        public MaintenanceRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }
    }
}
