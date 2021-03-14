using PhotoOrganizer.Common;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class Page
    {
        IBulkAttributeSetterService _bulkAttributeSetter;
        private IPageSizeService _pageSizeService;

        protected IPhotoLookupDataService _lookupDataService;
        protected IEventAggregator _eventAggregator;
        public ObservableCollection<PhotoNavigationItemViewModel> _navigationItems;
        public PhotoNavigationItemViewModel[] _cachedItems;

        public Page(
            IPhotoLookupDataService lookupDataService, 
            IEventAggregator eventAggregator,
            IBulkAttributeSetterService bulkAttributeSetter,
            IPageSizeService pageSizeService,
            ObservableCollection<PhotoNavigationItemViewModel> navigationItems)
        {
            _lookupDataService = lookupDataService;
            _eventAggregator = eventAggregator;
            _navigationItems = navigationItems;
            _bulkAttributeSetter = bulkAttributeSetter;
            _pageSizeService = pageSizeService;

            _cachedItems = new PhotoNavigationItemViewModel[_pageSizeService.PageSize];
            _navigationItems.CopyTo(_cachedItems, 0);
        }

        public async Task LoadFirstPage()
        {
            _pageSizeService.SetCurrentPageNumber(0);
            _pageSizeService.SetIsFirstPage(true);
            var itemNumber = await _lookupDataService.GetPhotoCountAsync();
            _pageSizeService.SetItemNumber(itemNumber);
            if (_pageSizeService.ItemNumber != 0)
            {
                await CreateNavigationViewModels();
            }

            _pageSizeService.SetAllPageNumber(_pageSizeService.ItemNumber / _pageSizeService.PageSize);
            _pageSizeService.SetIsLastPage(_pageSizeService.AllPageNumber == 0);
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
            var lookupItems = await _lookupDataService.GetPhotoFromBasedOnPageSizeAsync(_pageSizeService.CurrentPageNumber * _pageSizeService.PageSize, _pageSizeService.PageSize);

            _navigationItems.Clear();

            foreach (var item in lookupItems)
            {
                _navigationItems.Add(
                    new PhotoNavigationItemViewModel(
                        item.Id, item.DisplayMemberItem, item.PhotoPath, ColorMap.Map[item.ColorFlag],
                        nameof(PhotoDetailViewModel),
                        _eventAggregator, _bulkAttributeSetter));
            }
        }

        private async Task RefreshPageInformationsAsync(bool isDown)
        {
            if (isDown)
            {
                _pageSizeService.IncreaseCurrentPageNumber();
            }
            else
            {
                _pageSizeService.DecreaseCurrentPageNumber();
            }

            _pageSizeService.SetItemNumber(await _lookupDataService.GetPhotoCountAsync());
            _pageSizeService.SetAllPageNumber(_pageSizeService.ItemNumber / _pageSizeService.PageSize);

            if (_pageSizeService.CurrentPageNumber == 0)
            {
                _pageSizeService.SetIsFirstPage(true);
            }
            else { _pageSizeService.SetIsFirstPage(false); }

            if (_pageSizeService.CurrentPageNumber == _pageSizeService.AllPageNumber)
            {
                _pageSizeService.SetIsLastPage(true);
            }
            else { _pageSizeService.SetIsLastPage(false); }
        }
    }
}
