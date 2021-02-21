using PhotoOrganizer.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface ICacheService
    {
        Task LoadFirstAsync(ObservableCollection<NavigationItemViewModel> itemViewModels);
        Task LoadUpAsync(ObservableCollection<NavigationItemViewModel> itemViewModels);
        Task LoadDownAsync(ObservableCollection<NavigationItemViewModel> itemViewModels);
        bool CanMoveUp();
        bool CanMoveDown();
    }
}