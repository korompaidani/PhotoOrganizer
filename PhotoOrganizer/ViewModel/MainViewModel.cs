﻿using Autofac.Features.Indexed;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private IDirectoryReaderWrapperService _directoryReaderWrapperService;
        private IEventAggregator _eventAggregator;
        private ISettingsHandler _settingsHandler;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;
        private ILocationRepository _locationRepository;
        private WorkbenchViewModel _workbenchViewModel;

        public ICommand OpenWorkbenchCommand { get; set; }
        
        private object _selectedViewModel;

        public object SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { _selectedViewModel = value; OnPropertyChanged(); }
        }

        public INavigationViewModel NavigationViewModel { get; }

        public MainViewModel(INavigationViewModel navigationViewModel,
            IIndex<string, IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IDirectoryReaderWrapperService directoryReaderWrapperService,
            ILocationRepository locationRepository,
            ISettingsHandler settingsHandler)

        {
            NavigationViewModel = navigationViewModel;
            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;
            _directoryReaderWrapperService = directoryReaderWrapperService;
            _locationRepository = locationRepository;
            _eventAggregator = eventAggregator;
            _settingsHandler = settingsHandler;
            _eventAggregator.GetEvent<OpenPhotoViewEvent>().Subscribe(OnOpenPhotoView);
            _eventAggregator.GetEvent<OpenMapViewEvent>().Subscribe(OnOpenMapViewAsync);
            _eventAggregator.GetEvent<CloseMapViewEvent>().Subscribe(OnOpenWorkbenchView);
            _eventAggregator.GetEvent<OpenSettingsEvent>().Subscribe(OnOpenSettingsView);
            _eventAggregator.GetEvent<CloseSettingsEvent>().Subscribe(OnCloseSettingsView);

            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
        }

        private void OnCloseSettingsView(CloseSettingsEventArgs args)
        {
            SelectedViewModel = _workbenchViewModel;
        }

        private async void OnOpenSettingsView(OpenSettingsEventArgs args)
        {
            SelectedViewModel = new SettingsViewModel(_eventAggregator, _settingsHandler);
            await ((SettingsViewModel)SelectedViewModel).LoadAsync();
        }

        private async void OnOpenMapViewAsync(OpenMapViewEventArgs args)
        {
            SelectedViewModel = new MapViewModel(_eventAggregator, _messageDialogService, _locationRepository, args.PhotoId);
            await ((IDetailViewModel)SelectedViewModel).LoadAsync(args.Id);
        }

        private void OnOpenWorkbenchView(CloseMapViewEventArgs args)
        {
            SelectedViewModel = _workbenchViewModel;
        }

        private void OnOpenPhotoView(OpenPhotoViewEventArgs args)
        {
            SelectedViewModel = new PhotoViewModel(_eventAggregator, args.FullPath);
        }

        private void OnOpenWorkbench()
        {
            SelectedViewModel = _workbenchViewModel;
        }

        public async Task LoadWorkbenchAsync()
        {
            _workbenchViewModel = new WorkbenchViewModel(NavigationViewModel, _detailViewModelCreator, _eventAggregator, _messageDialogService, _directoryReaderWrapperService);
            await _workbenchViewModel.LoadAsync();
        }
    }
}
