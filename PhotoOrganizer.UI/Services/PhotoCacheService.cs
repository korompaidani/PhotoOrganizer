using Autofac;
using PhotoOrganizer.Common;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    // Caching logic can be implemented here. Now only one page is alive. First remove calls: CleanUpCache(); eg. when load up or down, 
    // based on a threashold the before x and after y items is stored also
    public class PhotoCacheService : CacheServiceBase
    {
        IBulkAttributeSetterService _bulkAttributeSetter;
        private IPageSizeService _pageSizeService;
        private IDictionary<int, Page> _pages;

        public PhotoCacheService(
            IPhotoLookupDataService lookupDataService, 
            IEventAggregator eventAggregator, 
            IBulkAttributeSetterService bulkAttributeSetter,
            IPageSizeService pageSizeService
            ) : base(lookupDataService, eventAggregator)
        {
            _pages = new Dictionary<int, Page>();
            _bulkAttributeSetter = bulkAttributeSetter;
            _pageSizeService = pageSizeService;
        }

        public override bool CanMoveDown()
        {
            return !_pageSizeService.IsLastPage;
        }

        public override bool CanMoveUp()
        {
            return !_pageSizeService.IsFirstPage;
        }

        public async override Task LoadDownAsync(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels)
        {
            if (CanMoveDown())
            {
                var nextPageIndex = _pageSizeService.CurrentPageNumber + 1;
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
            CleanUpCache(false);
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
                var nextPageIndex = _pageSizeService.CurrentPageNumber - 1;
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

        public void CleanUpCache(bool isSkipActual)
        {
            foreach (var page in _pages)
            {
                if (isSkipActual && page.Key == _pageSizeService.CurrentPageNumber)
                {
                    continue;
                }
                page.Value.KillThisPage();
            }

            _pages.Clear();
        }

        private Page CreatePage(ObservableCollection<PhotoNavigationItemViewModel> itemViewModels)
        {
            return new Page(_lookupDataService, _eventAggregator, _bulkAttributeSetter, _pageSizeService ,itemViewModels);
        }

        public override void SetViewModelForReload(INavigationViewModel navigation)
        {
            _pageSizeService.SetNavigationViewModel(navigation);
        }

        public override bool CleanCache()
        {
            try
            {
                foreach (var page in _pages)
                {
                    page.Value.KillThisPage();
                }

                _pages.Clear();
                return true;
            }
            catch (Exception ex)
            {
                var context = Bootstrapper.Container.Resolve<ApplicationContext>();
                string innerException = string.Empty;
                if (ex.InnerException != null && ex.InnerException.Message != null)
                {
                    innerException = ex.InnerException.Message;
                }
                context.AddErrorMessage(ErrorTypes.CacheError, ex.Message + innerException);
                return false;
            }
        }
    }
}
