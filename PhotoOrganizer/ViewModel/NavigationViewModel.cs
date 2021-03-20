using PhotoOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizer.UI.Data.Lookups;
using System.Windows.Input;
using Prism.Commands;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.Common;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        private IAlbumLookupDataService _albumLookupDataService;
        private IShelveLookupDataService _shelveLookupDataService;
        private IEventAggregator _eventAggregator;
        private ICacheService _cacheService;
        private IBulkAttributeSetterService _bulkAttributeSetter;

        public ICommand LoadDownNavigationCommand { get; }
        public ICommand LoadUpNavigationCommand { get; }
        public ObservableCollection<PhotoNavigationItemViewModel> Photos { get; set; }
        public ObservableCollection<AlbumNavigationItemViewModel> Albums { get; set; }
        public ObservableCollection<PhotoNavigationItemViewModel> ShelvePhotos { get; set; }

        public NavigationViewModel(
            IPhotoLookupDataService photoLookupDataService, 
            IAlbumLookupDataService albumLookupDataService,
            IShelveLookupDataService shelveLookupDataService,
            IEventAggregator eventAggregator,
            ICacheService cacheService,
            IBulkAttributeSetterService bulkAttributeSetter)
        {
            _photoLookupDataService = photoLookupDataService;
            _albumLookupDataService = albumLookupDataService;
            _shelveLookupDataService = shelveLookupDataService;
            _eventAggregator = eventAggregator;
            _cacheService = cacheService;
            _bulkAttributeSetter = bulkAttributeSetter;

            _cacheService.SetViewModelForReload(this);

            Photos = new ObservableCollection<PhotoNavigationItemViewModel>();
            Albums = new ObservableCollection<AlbumNavigationItemViewModel>();
            ShelvePhotos = new ObservableCollection<PhotoNavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
            _eventAggregator.GetEvent<AfterBulkSetPhotoDetailAttributesEvent>().Subscribe(AfterBulkSetPhotoDetailAttributes);
            _eventAggregator.GetEvent<AfterTabClosedEvent>().Subscribe(AfterTabsClosed);

            LoadDownNavigationCommand = new DelegateCommand(OnLoadNavigationDownExecute, OnLoadNavigationDownCanExecute);
            LoadUpNavigationCommand = new DelegateCommand(OnLoadNavigationUpExecute, OnLoadNavigationUpCanExecute);            
        }

        private void AfterTabsClosed(AfterTabClosedEventArgs args)
        {
            foreach(var tab in args.DetailInfo)
            {
                var afterSaveEventArgs = new AfterDetailSavedEventArgs
                {
                    Id = tab.Id,
                    Title = tab.Title,
                    PhotoPath = tab.FullFilePath,
                    ColorFlag = tab.ColorFlag,
                    ViewModelName = tab.ViewModelName,
                    IsShelveChanges = tab.IsShelveRelevant,
                    IsRemovingFromShelve = tab.IsShelveRelevant
                };
                AfterDetailSaved(afterSaveEventArgs);
            }
        }

        private void AfterBulkSetPhotoDetailAttributes(AfterBulkSetPhotoDetailAttributesEventArgs args)
        {
            foreach(var navigationAttribute in args.NavigationAttributes)
            {
                var lookupItem = Photos.SingleOrDefault(p => p.Id == navigationAttribute.Item1);
                lookupItem.DisplayMemberItem = navigationAttribute.Item2;
                lookupItem.ColorFlag = navigationAttribute.Item3;
                lookupItem.PhotoPath = navigationAttribute.Item4;
                
                lookupItem.SetOriginalColorFlag(navigationAttribute.Item3);
                lookupItem.IsChecked = false;
            }
        }

        public async Task LoadAsync()
        {
            await _cacheService.LoadFirstAsync(Photos);
            LoadShelve();

            var albums = await _albumLookupDataService.GetAlbumLookupAsync();
            Albums.Clear();
            foreach (var album in albums)
            {
                Albums.Add(new AlbumNavigationItemViewModel(album.Id, album.DisplayMemberItem, nameof(AlbumDetailViewModel), _eventAggregator));
            }
            
            ((DelegateCommand)LoadUpNavigationCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)LoadDownNavigationCommand).RaiseCanExecuteChanged();
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(PhotoDetailViewModel):
                    AfterDetailSavedForPhotos(Photos, args);
                    ReloadShelve(args);
                    break;
                case nameof(AlbumDetailViewModel):
                    AfterDetailSavedForAlbums(Albums, args);
                    break;
            }
        }

        private void AfterDetailSavedForPhotos(ObservableCollection<PhotoNavigationItemViewModel> items, AfterDetailSavedEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(p => p.Id == args.Id);
            if (lookupItem == null)
            {
                items.Add(new PhotoNavigationItemViewModel(args.Id, args.Title, args.PhotoPath, args.ViewModelName, args.ColorFlag, _eventAggregator, _bulkAttributeSetter));
            }
            else
            {
                lookupItem.DisplayMemberItem = args.Title;
                lookupItem.PhotoPath = args.PhotoPath;
                lookupItem.ColorFlag = args.ColorFlag;
            }
        }

        private void LoadShelve()
        {
            var shelveItems = _shelveLookupDataService.GetShelveLookup();
            ShelvePhotos.Clear();
            if (shelveItems != null)
            {
                foreach (var item in shelveItems)
                {
                    ShelvePhotos.Add(
                        new PhotoNavigationItemViewModel(
                            item.Id,
                            item.DisplayMemberItem,
                            item.PhotoPath,
                            ColorMap.Map[item.ColorFlag],
                            nameof(PhotoDetailViewModel),
                            _eventAggregator,
                            _bulkAttributeSetter
                            ));
                }
            }
        }

        private void ReloadShelve(AfterDetailSavedEventArgs args)
        {
            var shelveLookupItem = ShelvePhotos.SingleOrDefault(p => p.Id == args.Id);
            if (args.IsRemovingFromShelve && shelveLookupItem != null)
            {
                ShelvePhotos.Remove(shelveLookupItem);
                return;
            }                        

            if (shelveLookupItem == null)
            {
                if (args.IsShelveChanges)
                {
                    var photo = Photos.SingleOrDefault(p => p.Id == args.Id);
                    ShelvePhotos.Add(new PhotoNavigationItemViewModel(
                        photo.Id,
                        photo.DisplayMemberItem,
                        photo.PhotoPath,
                        photo.ColorFlag,
                        nameof(PhotoDetailViewModel),
                        _eventAggregator,
                        _bulkAttributeSetter
                        ));
                }                
            }
            else
            {
                shelveLookupItem.DisplayMemberItem = args.Title;
                shelveLookupItem.PhotoPath = args.PhotoPath;
                shelveLookupItem.ColorFlag = args.ColorFlag;
            }
        }

        private void AfterDetailSavedForAlbums(ObservableCollection<AlbumNavigationItemViewModel> items, AfterDetailSavedEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(p => p.Id == args.Id);
            if (lookupItem == null)
            {
                items.Add(new AlbumNavigationItemViewModel(args.Id, args.Title, args.ViewModelName, _eventAggregator));
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
                    AfterDetailDeletedForPhotos(Photos, args);
                    break;
                case nameof(AlbumDetailViewModel):
                    AfterDetailDeletedForAlbums(Albums, args);
                    break;
            }
        }

        private void AfterDetailDeletedForPhotos(ObservableCollection<PhotoNavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var item = items.SingleOrDefault(p => p.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        private void AfterDetailDeletedForAlbums(ObservableCollection<AlbumNavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var item = items.SingleOrDefault(p => p.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        private bool OnLoadNavigationUpCanExecute()
        {
            return _cacheService.CanMoveUp();
        }

        private async void OnLoadNavigationUpExecute()
        {
            await _cacheService.LoadUpAsync(Photos);
            ((DelegateCommand)LoadUpNavigationCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)LoadDownNavigationCommand).RaiseCanExecuteChanged();
        }        

        private bool OnLoadNavigationDownCanExecute()
        {
            return _cacheService.CanMoveDown();
        }

        private async void OnLoadNavigationDownExecute()
        {
            await _cacheService.LoadDownAsync(Photos);
            ((DelegateCommand)LoadUpNavigationCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)LoadDownNavigationCommand).RaiseCanExecuteChanged();
        }
    }
}
