using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        bool IsDisposed { get; set; }
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task SaveAsync();
        bool HasChanges();
        void Add(T model);
        void Remove(T model);
        void DisposeConnection();
    }
}
