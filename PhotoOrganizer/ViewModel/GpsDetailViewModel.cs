using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
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
    public class GpsDetailViewModel : DetailViewModelBase
    {
        private IGpsRepository _gpsRepository;
        private GpsWrapper _selectedGps;

        public ObservableCollection<GpsWrapper> GpsCollection { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public GpsWrapper SelectedGps
        {
            get { return _selectedGps; }
            set
            {
                _selectedGps = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public GpsDetailViewModel(IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService,
            IGpsRepository yearRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _gpsRepository = yearRepository;
            Title = "Gps";
            GpsCollection = new ObservableCollection<GpsWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        public async override Task LoadAsync(int id)
        {
            Id = id;
            foreach(var wrapper in GpsCollection)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            GpsCollection.Clear();

            var years = await _gpsRepository.GetAllAsync();

            foreach(var model in years)
            {
                var wrapper = new GpsWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                GpsCollection.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _gpsRepository.HasChanges();
            }
            if(e.PropertyName == nameof(GpsWrapper.HasErrors))
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
            return HasChanges && GpsCollection.All(y => !y.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            try
            {
                await _gpsRepository.SaveAsync();
                HasChanges = _gpsRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch(Exception ex)
            {
                while(ex.InnerException != null)
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
            return SelectedGps != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced = await _gpsRepository.IsReferencedByPhotoAsync(SelectedGps.Id);
            if (isReferenced)
            {
                await MessageDialogService.ShowInfoDialogAsync($"The GPS coordinate {SelectedGps.Title} can't be removed, as it is referenced by at least one photo");
                return;
            }

            SelectedGps.PropertyChanged -= Wrapper_PropertyChanged;
            _gpsRepository.Remove(SelectedGps.Model);
            GpsCollection.Remove(SelectedGps);
            SelectedGps = null;
            HasChanges = _gpsRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new GpsWrapper(new Gps());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _gpsRepository.Add(wrapper.Model);
            GpsCollection.Add(wrapper);

            // Trigger the validation
            wrapper.Title = "1900";
        }
    }
}
