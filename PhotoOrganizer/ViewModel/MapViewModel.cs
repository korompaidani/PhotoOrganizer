using PhotoOrganizer.UI.Event;
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
        private string _webUrl;
        private readonly object _lockObject = new object();
        public ICommand CloseMapCommand { get; }
        public ICommand OnSetCoordinatesOnPhotoOnlyCommand { get; }
        public ICommand OnSaveLocationCommand { get; }

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
             string coordinates)
            : base(eventAggregator, messageDialogService)
        {
            CloseMapCommand = new DelegateCommand(OnCloseMapAskCommand);
            OnSetCoordinatesOnPhotoOnlyCommand = new DelegateCommand<string>(OnSetCoordinateOnPhotoOnlyAndCloseCommand);
            OnSaveLocationCommand = new DelegateCommand<string>(OnSaveCoordinateOnPhotoOnlyAndCloseCommand, OnSaveCanExecute);
            _locationRepository = locationRepository;
        }

        private bool OnSaveCanExecute(string coordinateFromUrl)
        {
            return true;
        }

        private async void OnSaveCoordinateOnPhotoOnlyAndCloseCommand(string mapUrl)
        {
            //Location.Coordinates = mapUrl.TryConvertUrlToCoordinate();

            //var location = new Location { Coordinates = Location.Coordinates, LocationName = Location.LocationName };
            //_locationRepository.Add(location);

            //await _locationRepository.SaveAsync();
            
            //HasChanges = _locationRepository.HasChanges();
            //RaiseDetailSavedEvent(location.Id, location.LocationName);

            //EventAggregator.GetEvent<SaveCoordinatesEvent>().
            //    Publish(new SaveCoordinatesEventArgs
            //    {
            //        LocationId = location.Id,
            //        Coordinates = location.Coordinates,
            //        LocationName = location.LocationName
            //    });

            //EventAggregator.GetEvent<CloseMapViewEvent>().
            //    Publish(new CloseMapViewEventArgs());
        }

        private void OnSetCoordinateOnPhotoOnlyAndCloseCommand(string mapUrl)
        {
            EventAggregator.GetEvent<SetCoordinatesEvent>().
                Publish(new SetCoordinatesEventArgs
                {
                    Coordinates = mapUrl.TryConvertUrlToCoordinate()
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
            RaiseDetailSavedEvent(Location.Id, Location.LocationName);            
        }

        private Location CreateNewLocation()
        {
            var location = new Location();
            _locationRepository.Add(location);
            return location;
        }
    }
}
