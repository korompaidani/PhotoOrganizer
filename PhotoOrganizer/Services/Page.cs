using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class Page
    {
        public static int AllPageNumber;
        public static int CurrentPageNumber;
        public static bool IsFirstPage = false;
        public static bool IsLastPage = false;

        private static int PageSize = 42;
        private static int ItemNumber;

        protected IPhotoLookupDataService _lookupDataService;
        protected IEventAggregator _eventAggregator;
        public ObservableCollection<PhotoNavigationItemViewModel> _navigationItems;
        public PhotoNavigationItemViewModel[] _cachedItems;

        public Page(IPhotoLookupDataService lookupDataService, IEventAggregator eventAggregator, ObservableCollection<PhotoNavigationItemViewModel> navigationItems)
        {
            _lookupDataService = lookupDataService;
            _eventAggregator = eventAggregator;
            _navigationItems = navigationItems;

            _cachedItems = new PhotoNavigationItemViewModel[PageSize];
            _navigationItems.CopyTo(_cachedItems, 0);
        }

        public async Task LoadFirstPage()
        {
            CurrentPageNumber = 0;
            IsFirstPage = true;
            ItemNumber = await _lookupDataService.GetPhotoCountAsync();

            await CreateNavigationViewModels();
        }

        public async Task LoadUpPage()
        {
            await RefreshPageInformationsAsync(false);
            await CreateNavigationViewModels();
        }

        public async Task LoadDownPage()
        {            
            await RefreshPageInformationsAsync(true);
            await CreateNavigationViewModels();
        }

        public async Task LoadActualPage()
        {
            // TODO: it must be implemented when real caching works. This method will collaborate with cache to give the existing navi instead of creating new one
            await RefreshPageInformationsAsync(false);
            _navigationItems.Clear();
            foreach(var item in _cachedItems)
            {
                _navigationItems.Add(item);
            }
        }

        public void KillThisPage()
        {
            // TODO: disposing, cleaning, resetting must be implemented here
            if(_cachedItems != null)
            {
                foreach(var item in _cachedItems)
                {
                    // TODO: Dispose will be called here
                    if(item != null)
                    {
                        item.Picture = null;
                    }
                }
            }
            _cachedItems = null;
        }

        private async Task CreateNavigationViewModels()
        {
            var lookupItems = await _lookupDataService.GetPhotoFromBasedOnPageSizeAsync(CurrentPageNumber * PageSize, PageSize);

            _navigationItems.Clear();

            foreach (var item in lookupItems)
            {
                _navigationItems.Add(
                    new PhotoNavigationItemViewModel(
                        item.Id, item.DisplayMemberItem, item.PhotoPath, item.ColorFlag, 
                        nameof(PhotoDetailViewModel),
                        _eventAggregator));
            }
        }

        private async Task RefreshPageInformationsAsync(bool isDown)
        {
            if (isDown)
            {
                CurrentPageNumber++;
            }
            else
            {
                CurrentPageNumber--;
            }

            ItemNumber = await _lookupDataService.GetPhotoCountAsync();
            AllPageNumber = ItemNumber / PageSize;

            if (CurrentPageNumber == 0)
            {
                IsFirstPage = true;
            }
            else { IsFirstPage = false; }

            if (CurrentPageNumber == AllPageNumber)
            {
                IsLastPage = true;
            }
            else { IsLastPage = false; }
        }
    }
}
