using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Resources.Language;
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
            Title = TextResources.Location;
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
            await Save(false);
        }

        private async Task Save(bool isClosing, bool isOptimistic = true)
        {
            try
            {
                await _locationRepository.SaveAsync();

                if (!isClosing)
                {
                    HasChanges = _locationRepository.HasChanges();
                    RaiseCollectionSavedEvent();
                }                
            }
            catch (Exception ex)
            {
                if (!isClosing)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    await MessageDialogService.ShowInfoDialogAsync(TextResources.ErrorWhileSaveEntities_errorMessage +
                       TextResources.DataWillReloaded_message + ex.Message);

                    await LoadAsync(Id);
                }

                throw ex;
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
                await MessageDialogService.ShowInfoDialogAsync(String.Format(TextResources.LocationCantBeRemovedBecauseItIsUsedByEntity_message, SelectedLocation.LocationName));
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
            wrapper.LocationName = TextResources.NewPlace;
            wrapper.Coordinates = TextResources.DefaultCoordinates;
        }

        private async void AfterCoordinatesSavedAsNew(SaveCoordinatesAsNewEventArgs args)
        {
            await LoadAsync(args.LocationId);
        }

        private async void AfterCoordinatesSavedAsOverride(SaveCoordinatesAsOverrideEventArgs args)
        {
            await LoadAsync(args.LocationId);
        }

        public override async Task SaveChanges(bool isClosing, bool isOptimistic = true)
        {
            await Save(isClosing);
        }

        public override void DisposeConnection()
        {
            _locationRepository.DisposeConnection();
        }
    }
}
