using Autofac.Features.Indexed;
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
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;
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
            IDirectoryReaderWrapperService directoryReaderWrapperService)

        {
            NavigationViewModel = navigationViewModel;
            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;
            _directoryReaderWrapperService = directoryReaderWrapperService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPhotoViewEvent>().Subscribe(OnOpenPhotoView);
            _eventAggregator.GetEvent<OpenWorkbenchViewEvent>().Subscribe(OnOpenWorkbenchView);

            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
        }

        private void OnOpenWorkbenchView(OpenWorkbenchViewEventArgs args)
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
