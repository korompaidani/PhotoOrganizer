using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public abstract class CacheServiceBase : ICacheService
    {
        protected IPhotoLookupDataService _lookupDataService;
        protected IEventAggregator _eventAggregator;

        public CacheServiceBase(IPhotoLookupDataService lookupDataService, IEventAggregator eventAggregator)
        {
            _lookupDataService = lookupDataService;
            _eventAggregator = eventAggregator;
        }

        public abstract Task LoadItemsAsync(ObservableCollection<NavigationItemViewModel> itemViewModels);
    }
}
