using Autofac;
using Autofac.Features.Indexed;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ApplicationContext _context;
        private IMessageDialogService _messageDialogService;
        private IDirectoryReaderWrapperService _directoryReaderWrapperService;
        private IEventAggregator _eventAggregator;
        private ISettingsHandler _settingsHandler;
        private IPhotoMetaWrapperService _photoMetaWrapperService;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;
        private ILocationRepository _locationRepository;
        private WorkbenchViewModel _workbenchViewModel;

        public ICommand OpenWorkbenchCommand { get; set; }
        public ICommand OpenClosingAppCommand { get; set; }
        public ICommand OpenCancelClosingAppCommand { get; set; }

        private object _selectedViewModel;
        
        public bool CanClose { get; private set; }

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
            ISettingsHandler settingsHandler,
            IPhotoMetaWrapperService photoMetaWrapperService)

        {
            NavigationViewModel = navigationViewModel;
            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;
            _directoryReaderWrapperService = directoryReaderWrapperService;
            _locationRepository = locationRepository;
            _eventAggregator = eventAggregator;
            _settingsHandler = settingsHandler;
            _photoMetaWrapperService = photoMetaWrapperService;
            _context = Bootstrapper.Container.Resolve<ApplicationContext>();
            _eventAggregator.GetEvent<OpenPhotoViewEvent>().Subscribe(OnOpenPhotoView);
            _eventAggregator.GetEvent<OpenMapViewEvent>().Subscribe(OnOpenMapViewAsync);
            _eventAggregator.GetEvent<CloseMapViewEvent>().Subscribe(OnOpenWorkbenchView);
            _eventAggregator.GetEvent<OpenSettingsEvent>().Subscribe(OnOpenSettingsView);
            _eventAggregator.GetEvent<CloseSettingsEvent>().Subscribe(OnCloseSettingsView);
            
            CanClose = false;
            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
            OpenClosingAppCommand = new DelegateCommand(OnClosingApp);
            OpenCancelClosingAppCommand = new DelegateCommand(OnCancelClosingApp);
        }

        private void OnCancelClosingApp()
        {
        }

        private async void OnClosingApp()
        {           
            await _context.SaveAllOpenedDetailView();
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
            _workbenchViewModel = new WorkbenchViewModel(
                NavigationViewModel, 
                _detailViewModelCreator, 
                _eventAggregator, 
                _messageDialogService, 
                _directoryReaderWrapperService, 
                _photoMetaWrapperService);
            await _workbenchViewModel.LoadAsync();
        }
    }
}
