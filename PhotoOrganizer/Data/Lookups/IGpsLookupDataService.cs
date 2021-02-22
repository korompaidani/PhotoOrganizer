using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Lookups
{
    public interface IGpsLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetGpsLookupAsync();
    }
}