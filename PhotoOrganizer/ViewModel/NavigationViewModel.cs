using PhotoOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizer.UI.Data.Lookups;
using System.Windows.Input;
using Prism.Commands;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        private IAlbumLookupDataService _albumLookupDataService;
        private IEventAggregator _eventAggregator;
        
        public ICommand LoadNavigationCommand { get; }
        public ObservableCollection<NavigationItemViewModel> Photos { get; set; }
        public ObservableCollection<NavigationItemViewModel> Albums { get; set; }

        public NavigationViewModel(
            IPhotoLookupDataService photoLookupDataService, 
            IAlbumLookupDataService albumLookupDataService,
            IEventAggregator eventAggregator)
        {
            _photoLookupDataService = photoLookupDataService;
            _albumLookupDataService = albumLookupDataService;
            _eventAggregator = eventAggregator;
            Photos = new ObservableCollection<NavigationItemViewModel>();
            Albums = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            LoadNavigationCommand = new DelegateCommand(OnLoadNavigationExecute);
        }

        // TODO: Caching must be implemented here
        public async Task LoadAsync()
        {
            var photos = await _photoLookupDataService.GetPhotoLookupAsync();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(
                    new NavigationItemViewModel(
                        photo.Id, photo.DisplayMemberItem,
                        nameof(PhotoDetailViewModel), 
                        _eventAggregator));
            }

            var albums = await _albumLookupDataService.GetAlbumLookupAsync();
            Albums.Clear();
            foreach (var album in albums)
            {
                Albums.Add(new NavigationItemViewModel(album.Id, album.DisplayMemberItem, nameof(AlbumDetailViewModel), _eventAggregator));
            }
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(PhotoDetailViewModel):
                    AfterDetailSaved(Photos, args);
                    break;
                case nameof(AlbumDetailViewModel):
                    AfterDetailSaved(Albums, args);
                    break;
            }
        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSavedEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(p => p.Id == args.Id);
            if (lookupItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id, args.Title, args.ViewModelName, _eventAggregator));
            }
            else
            {
                lookupItem.DisplayMemberItem = args.Title;
            }
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(PhotoDetailViewModel):
                    AfterDetailDeleted(Photos, args);
                    break;
                case nameof(AlbumDetailViewModel):
                    AfterDetailDeleted(Albums, args);
                    break;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var item = items.SingleOrDefault(p => p.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        private async void OnLoadNavigationExecute()
        {
            await LoadAsync();
        }
    }
}
