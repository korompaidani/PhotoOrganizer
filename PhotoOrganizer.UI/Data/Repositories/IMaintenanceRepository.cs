using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IMaintenanceRepository : IGenericRepository<FileEntry>
    {
        Task RemoveAllEntriesFromTableAsync();
        Task<List<FileEntry>> GetAllFlaggedAsync();
        Task<List<FileEntry>> GetAllNonFlaggedAsync();
        Task<FileEntry> GetEntrybyOriginalPath(string originalPath);
        Task RemoveAllFlaggedEntriesFromTableAsync();
    }
}
