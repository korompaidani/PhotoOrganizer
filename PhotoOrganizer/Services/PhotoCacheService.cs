using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class PhotoCacheService : CacheServiceBase
    {
        private readonly int PageSize = 42;
        private int SkippedItem = 0;

        public PhotoCacheService(IPhotoLookupDataService lookupDataService, IEventAggregator eventAggregator) : base(lookupDataService, eventAggregator)
        {
        }

        public override async Task LoadItemsAsync(ObservableCollection<NavigationItemViewModel> photos)
        {
            var photoLookupItems = await _lookupDataService.GetPhotoFromBasedOnPageSizeAsync(SkippedItem, PageSize);
            SkippedItem += PageSize;
            // Create photos for up
            // Create photos for down
            // load the actual based on direction

            photos.Clear();
            
            foreach (var photo in photoLookupItems)
            {
                photos.Add(
                    new NavigationItemViewModel(
                        photo.Id, photo.DisplayMemberItem,
                        nameof(PhotoDetailViewModel),
                        _eventAggregator));
            }            
        }
    }
}
