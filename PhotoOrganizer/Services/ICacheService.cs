using PhotoOrganizer.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface ICacheService
    {
        void SetViewModelForReload(INavigationViewModel navigation);
        Task LoadFirstAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels);
        Task LoadUpAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels);
        Task LoadDownAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels);
        bool CanMoveUp();
        bool CanMoveDown();
    }
}