using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IPeopleRepository : IGenericRepository<People>
    {
        Task<bool> HasPeopleDisplayName(string displayName);
        Task<People> TryGetAnyPeopleByDisplayName(string displayName);
        Task<People> AddGetPeopleByUniqueDisplayNameAsync(string displayName);
        void AddAlias(Alias model);        
        Task<Alias> GetAliasByIdAsync(int id);
        Task<List<Alias>> GetAllAliasAsync();
    }
}
