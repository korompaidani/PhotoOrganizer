using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class MaintenanceRepository : GenericRepository<FileEntry, PhotoOrganizerDbContext>, IMaintenanceRepository
    {
        public MaintenanceRepository(PhotoOrganizerDbContext context) : base(context)
        {            
        }

        public async Task<List<FileEntry>> GetAllFlaggedAsync()
        {
            return await Context.FileEntries.Where(f => f.OriginalImagePath == "x").ToListAsync();
        }

        public async Task<List<FileEntry>> GetAllNonFlaggedAsync()
        {
            return await Context.FileEntries.Where(f => f.OriginalImagePath != "x").ToListAsync();
        }

        public async Task RemoveAllFlaggedEntriesFromTableAsync()
        {
            var entries = await GetAllAsync();
            foreach (var entry in entries)
            {
                if (entry.OriginalImagePath == "x")
                {
                    Context.FileEntries.Remove(entry);
                }                
            }

            await SaveAsync();
        }

        public async Task RemoveAllEntriesFromTableAsync()
        {
            var entries = await GetAllAsync();
            foreach (var entry in entries)
            {
                Context.FileEntries.Remove(entry);
            }

            await SaveAsync();
        }

        public async Task<FileEntry> GetEntrybyOriginalPath(string originalPath)
        {
            return await Context.FileEntries.FirstOrDefaultAsync(f => f.OriginalImagePath == originalPath);
        }
    }
}
