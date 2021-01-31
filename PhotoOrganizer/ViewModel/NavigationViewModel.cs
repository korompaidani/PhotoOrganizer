using PhotoOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizer.UI.Data.Lookups;
using System;
using PhotoOrganizer.UI.Data.Repositories;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<NavigationItemViewModel> Photos { get; set; }

        public NavigationViewModel(
            IPhotoLookupDataService photoLookupDataService, 
            IEventAggregator eventAggregator, 
            IPhotoRepository photoRepository) : base(photoRepository)
        {
            _photoLookupDataService = photoLookupDataService;
            _eventAggregator = eventAggregator;
            Photos = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterPhotoSavedEvent>().Subscribe(AfterPhotoSaved);
            _eventAggregator.GetEvent<AfterPhotoDeleteEvent>().Subscribe(AfterPhotoDeleted);
        }

        public async Task LoadAsync()
        {
            var photos = await _photoLookupDataService.GetPhotoLookupAsync();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(new NavigationItemViewModel(photo.Id, photo.DisplayMemberItem, _eventAggregator, _photoRepository));
            }
        }

        private void AfterPhotoSaved(AfterPhotoSavedEventArgs obj)
        {
            var lookupItem = Photos.SingleOrDefault(p => p.Id == obj.Id);
            if(lookupItem == null)
            {
                Photos.Add(new NavigationItemViewModel(obj.Id, obj.Title, _eventAggregator, _photoRepository));
            }
            else
            {
                lookupItem.DisplayMemberItem = obj.Title;
            }
        }

        private void AfterPhotoDeleted(int photoId)
        {
            var photo = Photos.SingleOrDefault(p => p.Id == photoId);
            if(photo != null)
            {
                Photos.Remove(photo);
            }
        }
    }
}
