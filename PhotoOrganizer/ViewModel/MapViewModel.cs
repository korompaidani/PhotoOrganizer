using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Engine;

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
            OnSetCoordinatesOnPhotoOnlyCommand = new DelegateCommand(OnSetCoordinateOnPhotoOnlyAndCloseCommand);
            SaveOverrideLocationCommand = new DelegateCommand(OnSaveOverrideLocationExecute, OnSaveOverrideLocationCanExecute);
            SaveAsNewLocationCommand = new DelegateCommand(OnSaveAsNewLocationCommandExecute, OnSaveAsNewLocationCommandCanExecute);
            _locationRepository = locationRepository;
            _photoId = photoId;
        }

        private async Task<string> RequestCoordinates()
        {
            var coordinates = await ChromiumBrowserEngine.Instance.RequestCoordinates();
            if (coordinates == null)
            {
                return String.Empty;
            }

            return coordinates;
        }

        private bool OnSaveAsNewLocationCommandCanExecute()
        {
            if (Location == null)
            {
                return false;
            }

            if (!Location.HasErrors)
            {
                return true;
            }
            return false;
        }

        private async void OnSaveAsNewLocationCommandExecute()
        {
            Location.Coordinates = await RequestCoordinates();
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
                _locationRepository.Add(location);

                await _locationRepository.SaveAsync();
                RaiseLocationChangedEvent(true, _photoId, location.Id, location.Coordinates);
                                
                EventAggregator.GetEvent<CloseMapViewEvent>().
                    Publish(new CloseMapViewEventArgs());

                await ChromiumBrowserEngine.Instance.RestoreMapDefaults();
            }
        }

        private void RaiseLocationChangedEvent(bool isNew, int photoId, int locationId, string coordinates)
        {
            if (isNew)
            {
                EventAggregator.GetEvent<SaveCoordinatesAsNewEvent>().Publish(
                new SaveCoordinatesAsNewEventArgs
                {
                    PhotoId = photoId,
                    LocationId = locationId,
                    Coordinates = coordinates,
                    ViewModelName = this.GetType().Name
                });
            }
            else
            {
                EventAggregator.GetEvent<SaveCoordinatesAsOverrideEvent>().Publish(
                new SaveCoordinatesAsOverrideEventArgs
                {
                    PhotoId = photoId,
                    LocationId = locationId,
                    Coordinates = coordinates,
                    ViewModelName = this.GetType().Name
                });
            }
        }

        private bool OnSaveOverrideLocationCanExecute()
        {
            if(Location == null)
            {
                return false;
            }

            if (!isNewLocationObject && !Location.HasErrors)
            {
                return true;
            }
            return false;
        }

        private async void OnSaveOverrideLocationExecute()
        {
            Location.Coordinates = await RequestCoordinates();
            var location = Location.Model;
            if (!Location.HasErrors)
            {
                await _locationRepository.SaveAsync();

                RaiseLocationChangedEvent(false, _photoId, location.Id, location.Coordinates);
                             
                EventAggregator.GetEvent<CloseMapViewEvent>().
                    Publish(new CloseMapViewEventArgs());

                await ChromiumBrowserEngine.Instance.RestoreMapDefaults();
            }
        }

        private async void OnSetCoordinateOnPhotoOnlyAndCloseCommand()
        {
            EventAggregator.GetEvent<SetCoordinatesEvent>().
                Publish(new SetCoordinatesEventArgs
                {
                    Coordinates = await RequestCoordinates(),
                    PhotoId = _photoId
                });            

            EventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());

            await ChromiumBrowserEngine.Instance.RestoreMapDefaults();
        }

        private async void OnCloseMapAskCommand()
        {
            // TODO: It must be removed to other commands
            // 1. SaveMapEvent --> user save the coordinate as a new location
            // 2. CloseMapEvent --> when the user just close the window (user must be asked about intention)
            // 3. SetCoordinatesOnMapEvent --> when the user dont save the location just set on photo (photo.coordinates will be persist of course)            

            EventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());

            await ChromiumBrowserEngine.Instance.RestoreMapDefaults();
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
            ((DelegateCommand)SaveOverrideLocationCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveAsNewLocationCommand).RaiseCanExecuteChanged();
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
            await Save();
        }

        private Location CreateNewLocation()
        {
            isNewLocationObject = true;
            var location = new Location();            
            return location;
        }

        public async override Task SaveChanges()
        {
            if(Location != null && !Location.HasErrors && HasChanges)
            {
                await Save();
            }
        }

        private async Task Save()
        {
            await _locationRepository.SaveAsync();
            HasChanges = _locationRepository.HasChanges();
        }
    }
}
