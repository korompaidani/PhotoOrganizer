using PhotoOrganizer.UI.ViewModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class PageSizeService : IPageSizeService
    {
        public int _allPageNumber;
        public int _currentPageNumber;
        public bool _isFirstPage = false;
        public bool _isLastPage = false;
        public int _pageSize = 42;
        public int _itemNumber;
        private INavigationViewModel _navigationViewModel;

        public int AllPageNumber => _allPageNumber;
        public int CurrentPageNumber => _currentPageNumber;
        public bool IsFirstPage => _isFirstPage;
        public bool IsLastPage => _isLastPage;

        public int PageSize => _pageSize;
        public int ItemNumber => _itemNumber;

        public async Task SetPageSize(int size)
        {
            _pageSize = 42;
            if(_navigationViewModel != null)
            {
                await _navigationViewModel.LoadAsync();
            }
        }

        public void SetAllPageNumber(int number)
        {
            _allPageNumber = number;
        }

        public void SetCurrentPageNumber(int number)
        {
            _currentPageNumber = number;
        }

        public void IncreaseCurrentPageNumber()
        {
            _currentPageNumber++;
        }

        public void DecreaseCurrentPageNumber()
        {
            _currentPageNumber--;
        }

        public void SetIsFirstPage(bool isFirst)
        {
            _isFirstPage = isFirst;
        }

        public void SetIsLastPage(bool isLast)
        {
            _isLastPage = isLast;
        }

        public void SetItemNumber(int number)
        {
            _itemNumber = number;
        }

        public void SetNavigationViewModel(INavigationViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
        }
    }
}
