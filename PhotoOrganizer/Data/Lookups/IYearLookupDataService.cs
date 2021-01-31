using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Lookups
{
    public interface IYearLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetYearLookupAsync();
    }
}