using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public interface INavigationViewModel
    {
        Task LoadAsync();
        Task<bool> ClearNavigationIfEmptyAsync();
    }
}