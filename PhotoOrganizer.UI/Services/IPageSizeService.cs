using PhotoOrganizer.UI.ViewModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface IPageSizeService
    {
        int AllPageNumber { get; }
        int CurrentPageNumber { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        int PageSize { get; }
        int ItemNumber { get; }

        void SetAllPageNumber(int number);
        void SetCurrentPageNumber(int number);
        void IncreaseCurrentPageNumber();
        void DecreaseCurrentPageNumber();
        void SetIsFirstPage(bool isFirst);
        void SetIsLastPage(bool isLast);
        void SetItemNumber(int number);

        void SetNavigationViewModel(INavigationViewModel navigationViewModel);
        Task SetPageSize(int size);
    }
}