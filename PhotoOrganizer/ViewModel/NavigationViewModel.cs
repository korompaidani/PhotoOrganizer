using PhotoOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizer.UI.Data.Lookups;
using System;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<NavigationItemViewModel> Photos { get; set; }

        public NavigationViewModel(IPhotoLookupDataService photoLookupDataService, IEventAggregator eventAggregator)
        {
            _photoLookupDataService = photoLookupDataService;
            _eventAggregator = eventAggregator;
            Photos = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
        }

        public async Task LoadAsync()
        {
            var photos = await _photoLookupDataService.GetPhotoLookupAsync();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(new NavigationItemViewModel(photo.Id, photo.DisplayMemberItem, nameof(PhotoDetailViewModel), _eventAggregator));
            }
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs obj)
        {
            switch (obj.ViewModelName)
            {
                case nameof(PhotoDetailViewModel):
                    var lookupItem = Photos.SingleOrDefault(p => p.Id == obj.Id);
                    if (lookupItem == null)
                    {
                        Photos.Add(new NavigationItemViewModel(obj.Id, obj.Title, nameof(PhotoDetailViewModel), _eventAggregator));
                    }
                    else
                    {
                        lookupItem.DisplayMemberItem = obj.Title;
                    }
                    break;
            }
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(PhotoDetailViewModel):
                    var photo = Photos.SingleOrDefault(p => p.Id == args.Id);
                    if (photo != null)
                    {
                        Photos.Remove(photo);
                    }
                    break;
            }
        }
    }
}
