using System.Collections.Generic;

namespace PhotoOrganizer.UI.Services
{
    public interface IBulkAttributeSetterService
    {
        bool IsCheckedById(int photoNavigationLookupId);
        bool IsAnySelectedItem(int? exceptId = null);
        void SetCheckedStateForId(int photoNavigationLookupId, bool checkStatus);
        bool HasPreviousSelectionEntries();
        HashSet<int> GetPreviousSelection();
    }
}