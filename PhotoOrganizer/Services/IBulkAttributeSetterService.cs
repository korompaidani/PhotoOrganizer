namespace PhotoOrganizer.UI.Services
{
    public interface IBulkAttributeSetterService
    {
        bool IsCheckedById(int photoNavigationLookupId);
        bool IsAnySelectedItem(int? exceptId);
        void SetCheckedStateForId(int photoNavigationLookupId, bool checkStatus);        
    }
}