using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public interface IDetailViewModel
    {
        bool HasChanges { get; }
        Task LoadAsync(int? id);
    }
}