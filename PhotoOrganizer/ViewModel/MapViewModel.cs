﻿using PhotoOrganizer.UI.Event;
using PhotoOrganizer.MapTools;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MapViewModel : DetailViewModelBase, IMapViewModel
    {
        private LocationWrapper _location;
        private ILocationRepository _locationRepository;
        private int _photoId;
        private string _webUrl;
        private string _originalLocationName;
        private string _originalCoordinates;
        private readonly object _lockObject = new object();
        private bool isNewLocationObject = false;
        public ICommand CloseMapCommand { get; }
        public ICommand OnSetCoordinatesOnPhotoOnlyCommand { get; }
        public ICommand SaveOverrideLocationCommand { get; }
        public ICommand SaveAsNewLocationCommand { get; }

        public string WebUrl
        {
            get
            {
                lock (_lockObject)
                {
                    return _webUrl;
                }
            }
            set
            {
                lock(_lockObject)
                {
                    _webUrl = value;
                }
            }
        }

        public LocationWrapper Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }

        public MapViewModel(
             IEventAggregator eventAggregator,
             IMessageDialogService messageDialogService,
             ILocationRepository locationRepository,
             int photoId)
            : base(eventAggregator, messageDialogService)
        {
            CloseMapCommand = new DelegateCommand(OnCloseMapAskCommand);
            OnSetCoordinatesOnPhotoOnlyCommand = new DelegateCommand<string>(OnSetCoordinateOnPhotoOnlyAndCloseCommand);
            SaveOverrideLocationCommand = new DelegateCommand<string>(OnSaveOverrideLocationExecute, OnSaveOverrideLocationCanExecute);
            SaveAsNewLocationCommand = new DelegateCommand<string>(OnSaveAsNewLocationCommandExecute, OnSaveAsNewLocationCommandCanExecute);
            _locationRepository = locationRepository;
            _photoId = photoId;
        }

        private bool OnSaveAsNewLocationCommandCanExecute(string mapUrl)
        {
            if (!Location.HasErrors)
            {
                return true;
            }
            return false;
        }

        private void OnSaveAsNewLocationCommandExecute(string mapUrl)
        {
            Location.Coordinates = mapUrl.TryConvertUrlToCoordinate();
            var location = Location.Model;
            if (!Location.HasErrors)
            {
                if (!isNewLocationObject)
                {
                    var newLocation = CreateNewLocation();
                    newLocation.Coordinates = Location.Coordinates;
                    newLocation.LocationName = Location.LocationName;
                    Location.LocationName = _originalLocationName;
                    Location.Coordinates = _originalCoordinates;
                    location = newLocation;
                }
                else
                {
                }

                _locationRepository.Save();
                RaiseDetailSavedEvent(_photoId, location.Id);

                EventAggregator.GetEvent<CloseMapViewEvent>().
                    Publish(new CloseMapViewEventArgs());
            }
        }

        private void RaiseDetailSavedEvent(int photoId, int locationId)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs
                {
                    Id = photoId,
                    LocationId = locationId,
                    ViewModelName = this.GetType().Name
                });
        }

        private bool OnSaveOverrideLocationCanExecute(string mapUrl)
        {
            if (!isNewLocationObject && !Location.HasErrors)
            {
                return true;
            }
            return false;
        }

        private void OnSaveOverrideLocationExecute(string mapUrl)
        {
            Location.Coordinates = mapUrl.TryConvertUrlToCoordinate();
            var location = Location.Model;
            if (!Location.HasErrors)
            {
                _locationRepository.Save();
                RaiseDetailSavedEvent(_photoId, location.Id);

                EventAggregator.GetEvent<CloseMapViewEvent>().
                    Publish(new CloseMapViewEventArgs());
            }
        }

        private void OnSetCoordinateOnPhotoOnlyAndCloseCommand(string mapUrl)
        {
            EventAggregator.GetEvent<SetCoordinatesEvent>().
                Publish(new SetCoordinatesEventArgs
                {
                    Coordinates = mapUrl.TryConvertUrlToCoordinate(),
                    PhotoId = _photoId
                });

            EventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());
        }

        private void OnCloseMapAskCommand()
        {
            // TODO: It must be removed to other commands
            // 1. SaveMapEvent --> user save the coordinate as a new location
            // 2. CloseMapEvent --> when the user just close the window (user must be asked about intention)
            // 3. SetCoordinatesOnMapEvent --> when the user dont save the location just set on photo (photo.coordinates will be persist of course)

            EventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());
        }

        public async override Task LoadAsync(int locationId)
        {
            // TODO if locationId exist than web url source must be changed to a real coordinate

            var location = locationId > 0
                ? await _locationRepository.GetByIdAsync(locationId)
                : CreateNewLocation();

            _originalLocationName = location.LocationName;
            _originalCoordinates = location.Coordinates;
            // TODO: consider that always new must be created here field does not display the name, but the map actual state reflects the incoming coordinate info if there is any

            Location = new LocationWrapper(location);

            Location.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _locationRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Location.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Location.Id == 0)
            {
                Location.LocationName = "";
                Location.Coordinates = "";
            }

            Location.PropertyChanged += Location_PropertyChanged;
        }

        private void Location_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand<string>)SaveOverrideLocationCommand).RaiseCanExecuteChanged();
            ((DelegateCommand<string>)SaveAsNewLocationCommand).RaiseCanExecuteChanged();
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return Location != null && !Location.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await _locationRepository.SaveAsync();
            HasChanges = _locationRepository.HasChanges();        
        }

        private Location CreateNewLocation()
        {
            isNewLocationObject = true;
            var location = new Location();
            _locationRepository.Add(location);
            return location;
        }
    }
}
