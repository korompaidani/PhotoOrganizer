﻿using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoDetailViewModel : DetailViewModelBase, IPhotoDetailViewModel
    {
        private PhotoWrapper _photo;
        
        private ILocationLookupDataService _locationLookupDataService;
        private IPhotoRepository _photoRepository;
        private ILocationRepository _locationRepository;
        private IPeopleRepository _peopleRepository;

        public ICommand OpenPhotoCommand { get; }
        public ICommand OpenMapCommand { get; }
        public ICommand OpenPeopleAddViewCommand { get; }

        public ObservableCollection<LookupItem> Locations { get; }
        public ObservableCollection<PeopleWrapper> Peoples { get; }

        public PhotoWrapper Photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public PhotoDetailViewModel(
            IPhotoRepository photoRepository,
            ILocationRepository locationRepository,
            IPeopleRepository peopleRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            ILocationLookupDataService locationLookupDataService
            ) : base(eventAggregator, messageDialogService)
        {
            _photoRepository = photoRepository;
            _locationRepository = locationRepository;
            _peopleRepository = peopleRepository;
            _locationLookupDataService = locationLookupDataService;

            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Subscribe(AfterCollectionSaved);
            EventAggregator.GetEvent<CloseMapViewEvent>()
                .Subscribe(AfterMapViewClosed);

            OpenPhotoCommand = new DelegateCommand(OnOpenPhoto);
            OpenMapCommand = new DelegateCommand(OnOpenMap);
            OpenPeopleAddViewCommand = new DelegateCommand(OnOpenPeopleAddView);

            Locations = new ObservableCollection<LookupItem>();
            Peoples = new ObservableCollection<PeopleWrapper>();            
        }

        public override async Task LoadAsync(int photoId)
        {
            var photo = photoId > 0
                ? await _photoRepository.GetByIdAsync(photoId)
                : CreateNewPhoto();

            Id = photoId;

            InitializePhoto(photo);

            InitializePeople(photo.Peoples);

            await LoadLocationLookupAsync();
        }

        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if(args.ViewModelName == nameof(LocationDetailViewModel))
            {
                await LoadLocationLookupAsync();
            }
        }

        private async void AfterMapViewClosed(CloseMapViewEventArgs args)
        {
            var location = new Location { LocationName = args.LocationName, Coordinates = args.Coordinates };
            _locationRepository.Add(location);
            await _locationRepository.SaveAsync();
            Photo.LocationId = location.Id;
            Locations.Add(new LookupItem { Id = location.Id, DisplayMemberItem = location.LocationName });
        }

        private void OnOpenPhoto()
        {
            EventAggregator.GetEvent<OpenPhotoViewEvent>().
                Publish(
                    new OpenPhotoViewEventArgs
                    {
                        Id = Id,
                        FullPath = Photo.FullPath,
                        ViewModelName = "PhotoViewModel"
                    });
        }

        private async void OnOpenMap()
        {
            var locationId = Photo.LocationId;
            string coordinate = null;
            if (locationId != null)
            {
                coordinate = await _locationRepository.TryGetCoordinatesByIdAsync((int)locationId);                
            }
            
            EventAggregator.GetEvent<OpenMapViewEvent>().
                Publish(
                    new OpenMapViewEventArgs
                    {
                        PhotoId = Id,
                        Coordinates = coordinate, //nullcheck on the other side
                        ViewModelName = "MapViewModel"
                    });
        }

        private void OnOpenPeopleAddView()
        {
            var window = new PeopleSelectionCreationView();
            window.DataContext = new PeopleSelectionCreationViewModel(_photoRepository, _peopleRepository, Peoples, this);
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void InitializePeople(ICollection<People> peoples)
        {
            foreach(var people in Peoples)
            {
                people.PropertyChanged -= PeopleWrapper_PropertyChanged;
            }
            Peoples.Clear();
            foreach(var people in peoples)
            {
                var wrapper = new PeopleWrapper(people);
                Peoples.Add(wrapper);
                wrapper.PropertyChanged += PeopleWrapper_PropertyChanged;
            }
        }

        public void PeopleWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _photoRepository.HasChanges();
            }
            if(e.PropertyName == nameof(PeopleWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void InitializePhoto(Photo photo)
        {
            Photo = new PhotoWrapper(photo);
            Photo.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _photoRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Photo.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if(e.PropertyName == nameof(Photo.Title))
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Photo.Id == 0)
            {
                Photo.Title = "";
                Photo.FullPath = "";
            }

            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Photo.Title}";
        }

        private async Task LoadLocationLookupAsync()
        {
            Locations.Clear();
            Locations.Add(new NullLookupItem { DisplayMemberItem = "-" });
            var lookup = await _locationLookupDataService.GetLocationLookupAsync();
            foreach (var lookupItem in lookup)
            {
                Locations.Add(lookupItem);
            }
        }

        // TODO: refactor if PhotoService layer has been created this should be moved there
        private Photo CreateNewPhoto()
        {
            var photo = new Photo();
            _photoRepository.Add(photo);
            return photo;
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync, 
                () => 
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}");
                });           
        }

        protected override bool OnSaveCanExecute()
        {
            return Photo != null 
                && !Photo.HasErrors 
                && Peoples.All(p => !p.HasErrors)
                && HasChanges;
        }

        protected override async void OnDeleteExecute()
        {
            if(await _photoRepository.HasAlbums(Photo.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync($"{Photo.Title} can't be deleted as it is part of at least one album.");
                return;
            }

            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete {Photo.Title}?", "Question");
            if(result == MessageDialogResult.Ok)
            {
                _photoRepository.Remove(Photo.Model);
                await _photoRepository.SaveAsync();
                RaiseDetailDeletedEvent(Photo.Id);                
            }
        }
    }
}
