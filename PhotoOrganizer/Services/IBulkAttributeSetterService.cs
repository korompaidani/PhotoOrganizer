namespace PhotoOrganizer.UI.Services
{
    public interface IBulkAttributeSetterService
    {
        bool IsCheckedById(int photoNavigationLookupId);
        void SetCheckedStateForId(int photoNavigationLookupId, bool checkStatus);
    }
}