using PhotoOrganizer.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface ICacheService
    {
        Task LoadItemsAsync(ObservableCollection<NavigationItemViewModel> itemViewModels);
    }
}