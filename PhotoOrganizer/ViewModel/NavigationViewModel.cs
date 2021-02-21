﻿using PhotoOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizer.UI.Data.Lookups;
using System.Windows.Input;
using Prism.Commands;
using PhotoOrganizer.UI.Services;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        private IAlbumLookupDataService _albumLookupDataService;
        private IEventAggregator _eventAggregator;
        private ICacheService _cacheService;

        public ICommand LoadDownNavigationCommand { get; }
        public ICommand LoadUpNavigationCommand { get; }
        public ObservableCollection<NavigationItemViewModel> Photos { get; set; }
        public ObservableCollection<NavigationItemViewModel> Albums { get; set; }

        public NavigationViewModel(
            IPhotoLookupDataService photoLookupDataService, 
            IAlbumLookupDataService albumLookupDataService,
            IEventAggregator eventAggregator,
            ICacheService cacheService)
        {
            _photoLookupDataService = photoLookupDataService;
            _albumLookupDataService = albumLookupDataService;
            _eventAggregator = eventAggregator;
            _cacheService = cacheService;
            Photos = new ObservableCollection<NavigationItemViewModel>();
            Albums = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            LoadDownNavigationCommand = new DelegateCommand(OnLoadNavigationDownExecute, OnLoadNavigationDownCanExecute);
            LoadUpNavigationCommand = new DelegateCommand(OnLoadNavigationUpExecute, OnLoadNavigationUpCanExecute);
        }        

        public async Task LoadAsync()
        {
            await _cacheService.LoadFirstAsync(Photos);
            
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

        private bool OnLoadNavigationUpCanExecute()
        {
            return _cacheService.CanMoveUp();
        }

        private async void OnLoadNavigationUpExecute()
        {
            await _cacheService.LoadUpAsync(Photos);
        }        

        private bool OnLoadNavigationDownCanExecute()
        {
            return _cacheService.CanMoveDown();
        }

        private async void OnLoadNavigationDownExecute()
        {
            await _cacheService.LoadDownAsync(Photos);
        }
    }
}
