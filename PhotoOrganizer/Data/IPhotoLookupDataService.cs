using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.Data
{
    public interface IPhotoLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetPhotoLookupAsync();
    }
}