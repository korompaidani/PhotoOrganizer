using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class LocationDetailViewModel : DetailViewModelBase
    {
        private ILocationRepository _locationRepository;
        private LocationWrapper _selectedLocation;

        public ObservableCollection<LocationWrapper> Locations { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public LocationWrapper SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                _selectedLocation = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public LocationDetailViewModel(IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService,
            ILocationRepository locationRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _locationRepository = locationRepository;
            Title = "Location";
            Locations = new ObservableCollection<LocationWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);

            EventAggregator.GetEvent<SaveCoordinatesAsNewEvent>().Subscribe(AfterCoordinatesSavedAsNew);
            EventAggregator.GetEvent<SaveCoordinatesAsOverrideEvent>().Subscribe(AfterCoordinatesSavedAsOverride);
        }        

        public async override Task LoadAsync(int id)
        {
            Id = id;
            foreach(var wrapper in Locations)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            Locations.Clear();

            var locations = await _locationRepository.GetAllAsync();

            foreach(var model in locations)
            {
                var wrapper = new LocationWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                Locations.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _locationRepository.HasChanges();
            }
            if(e.PropertyName == nameof(LocationWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && Locations.All(y => !y.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            await Save();
        }

        private async Task Save()
        {
            try
            {
                await _locationRepository.SaveAsync();
                HasChanges = _locationRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                await MessageDialogService.ShowInfoDialogAsync("Error while saving the entities, " +
                    "the data will be reloaded. Details: " + ex.Message);
                await LoadAsync(Id);
            }
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedLocation != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced = await _locationRepository.IsReferencedByPhotoAsync(SelectedLocation.Id);
            if (isReferenced)
            {
                await MessageDialogService.ShowInfoDialogAsync($"The location {SelectedLocation.LocationName} can't be removed, as it is referenced by at least one photo");
                return;
            }

            SelectedLocation.PropertyChanged -= Wrapper_PropertyChanged;
            _locationRepository.Remove(SelectedLocation.Model);
            Locations.Remove(SelectedLocation);
            SelectedLocation = null;
            HasChanges = _locationRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new LocationWrapper(new Location());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _locationRepository.Add(wrapper.Model);
            Locations.Add(wrapper);

            // Trigger the validation
            wrapper.LocationName = "new place";
            wrapper.Coordinates = "46.84172812451524, 16.84248724161438";
        }

        private async void AfterCoordinatesSavedAsNew(SaveCoordinatesAsNewEventArgs args)
        {
            await LoadAsync(args.LocationId);
        }

        private async void AfterCoordinatesSavedAsOverride(SaveCoordinatesAsOverrideEventArgs args)
        {
            await LoadAsync(args.LocationId);
        }

        public override async Task SaveChanges()
        {
            await Save();
        }
    }
}
