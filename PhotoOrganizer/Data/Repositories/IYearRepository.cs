using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IYearRepository : IGenericRepository<Year>
    {
        Task<bool> IsReferencedByPhotoAsync(int yearId);
    }
}