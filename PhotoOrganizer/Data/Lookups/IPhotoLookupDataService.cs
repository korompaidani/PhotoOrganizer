using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Lookups
{
    public interface IPhotoLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetPhotoLookupAsync();
        Task<IEnumerable<LookupItem>> GetPhotoFromBasedOnPageSizeAsync(int from, int pageSize);
    }
}