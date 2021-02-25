using Autofac.Features.Indexed;
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

        public ICommand OpenPhoto { get; set; }
        public ICommand OpenWorkbench { get; set; }
        
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

            OpenPhoto = new DelegateCommand(OnOpenPhoto);
            OpenWorkbench = new DelegateCommand(OnOpenWorkbench);
        }

        private void OnOpenPhoto()
        {
            SelectedViewModel = new PhotoViewModel();
        }

        private async void OnOpenWorkbench()
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
