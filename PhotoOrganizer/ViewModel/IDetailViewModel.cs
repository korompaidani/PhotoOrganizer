using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public interface IDetailViewModel
    {
        bool HasChanges { get; }
        int Id { get; }
        Task LoadAsync(int id);
        Task SaveChanges(bool isClosing, bool isOptimistic = true);
    }
}