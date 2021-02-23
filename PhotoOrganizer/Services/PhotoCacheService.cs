using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    // Caching logic can be implemented here. Now only one page is alive. First remove calls: CleanUpCache();
    public class PhotoCacheService : CacheServiceBase
    {
        private IDictionary<int, Page> _pages;

        public PhotoCacheService(IPhotoLookupDataService lookupDataService, IEventAggregator eventAggregator) : base(lookupDataService, eventAggregator)
        {
            _pages = new Dictionary<int, Page>();
        }

        public override bool CanMoveDown()
        {
            return !Page.IsLastPage;
        }

        public override bool CanMoveUp()
        {
            return !Page.IsFirstPage;
        }

        public async override Task LoadDownAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels)
        {
            if (CanMoveDown())
            {
                var nextPageIndex = Page.CurrentPageNumber + 1;
                if (!_pages.ContainsKey(nextPageIndex))
                {
                    // create new
                    var page = CreatePage(itemViewModels);
                    await page.LoadDownPage();
                    _pages.Add(nextPageIndex, page);
                }
                else
                {
                    // load existing
                    Page existingPage = null;
                    _pages.TryGetValue(nextPageIndex, out existingPage);
                    await existingPage.LoadActualPage();
                }
            }

            CleanUpCache(false);
        }

        public async override Task LoadFirstAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels)
        {
            if (!_pages.ContainsKey(0))
            {
                var page = CreatePage(itemViewModels);
                await page.LoadFirstPage();
                _pages.Add(0, page);
            }
            else
            {
                // load existing
                Page existingPage = null;
                _pages.TryGetValue(0, out existingPage);
                await existingPage.LoadActualPage();
            }
        }

        public async override Task LoadUpAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels)
        {
            if (CanMoveUp())
            {
                var nextPageIndex = Page.CurrentPageNumber - 1;
                if (!_pages.ContainsKey(nextPageIndex))
                {
                    // create new
                    var page = CreatePage(itemViewModels);
                    await page.LoadUpPage();
                    _pages.Add(nextPageIndex, page);
                }
                else
                {
                    // load existing
                    Page existingPage = null;
                    _pages.TryGetValue(nextPageIndex, out existingPage);
                    await existingPage.LoadActualPage();
                }
            }

            CleanUpCache(false);
        }

        private Page CreatePage(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels)
        {
            return new Page(_lookupDataService, _eventAggregator, itemViewModels);
        }

        private void CleanUpCache(bool isExceptActual)
        {
            foreach (var page in _pages)
            {
                if (isExceptActual && page.Key == Page.CurrentPageNumber)
                {
                    continue;
                }
                page.Value.KillThisPage();
            }

            _pages.Clear();
        }
    }
}
