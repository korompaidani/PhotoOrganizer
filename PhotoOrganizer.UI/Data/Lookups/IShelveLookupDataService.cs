using PhotoOrganizer.Model;
using System.Collections.Generic;

namespace PhotoOrganizer.UI.Data.Lookups
{
    public interface IShelveLookupDataService
    {
        List<LookupItem> GetShelveLookup();
    }
}