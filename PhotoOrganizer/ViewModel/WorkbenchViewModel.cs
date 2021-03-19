using Autofac;
using Autofac.Features.Indexed;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.View;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class WorkbenchViewModel : ViewModelBase
    {
        private ApplicationContext _context;
        private IDetailViewModel _selectedDetailViewModel;
        private IMessageDialogService _messageDialogService;
        private IDirectoryReaderWrapperService _directoryReaderWrapperService;
        private IEventAggregator _eventAggregator;

        private int nextNewItemId = 0;

        public ICommand CreateNewDetailCommand { get; }
        public ICommand OpenSingleDetailViewCommand { get; }
        public ICommand OpenSettingsViewCommand { get; }
        public ICommand CreatePhotosFromLibraryCommand { get; }
        public ICommand WriteAllSavedMetadataCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }

        private IIndex<string, IDetailViewModel> _detailViewModelCreator;
        private IPhotoMetaWrapperService _photoMetaWrapperService;

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel
        {
            get
            {
                return _selectedDetailViewModel;
            }
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public WorkbenchViewModel(
            INavigationViewModel navigationViewModel,
            IIndex<string, IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IDirectoryReaderWrapperService directoryReaderWrapperService,
            IPhotoMetaWrapperService photoMetaWrapperService)
        {
            NavigationViewModel = navigationViewModel;

            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;
            _directoryReaderWrapperService = directoryReaderWrapperService;
            _photoMetaWrapperService = photoMetaWrapperService;
            _context = Bootstrapper.Container.Resolve<ApplicationContext>();

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().
                Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().
                Subscribe(AfterDetailDeleted);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().
                Subscribe(AfterDetailClosed);
            _eventAggregator.GetEvent<CloseProgressWindowEvent>().
                Subscribe(AfterProgressWindowClosed);

            DetailViewModels = new ObservableCollection<IDetailViewModel>();
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
            CreatePhotosFromLibraryCommand = new DelegateCommand(OnCreatePhotosFromLibraryExecute);
            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailExecute);
            OpenSettingsViewCommand = new DelegateCommand(OnOpenSettingsViewExecute);
            WriteAllSavedMetadataCommand = new DelegateCommand(OnWriteAllSavedMetadataCommand);
        }

        private async void AfterProgressWindowClosed(CloseProgressWindowEventArgs args)
        {
            await NavigationViewModel.LoadAsync();
        }

        private void OnWriteAllSavedMetadataCommand()
        {
            var window = new WritingToFileView();
            var peopleSelectionViewModel = new WritingToFileViewModel(_eventAggregator, _photoMetaWrapperService);

            window.DataContext = peopleSelectionViewModel;
            window.Owner = Application.Current.MainWindow;

            window.ShowDialog();
        }

        private void OnOpenSettingsViewExecute()
        {
            _eventAggregator.GetEvent<OpenSettingsEvent>().
                Publish(
                    new OpenSettingsEventArgs
                    {
                    });
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnCreatePhotosFromLibraryExecute()
        {
            // TODO:            
            // 1.       Detect that database has entries
            // 2.       if yes: ask to save --> and if yes than save
            // 3. DONE  Delete all data from photo
            // 4. DONE  Read data from library
            // 5.       show progressbar during load

            await _directoryReaderWrapperService.LoadAllFromLibraryAsync();
            await LoadAsync();
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id
                && vm.GetType().Name == args.ViewModelName);

            if (detailViewModel == null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                try
                {
                    await detailViewModel.LoadAsync(args.Id);
                    _context.RegisterOpenedDetailView(detailViewModel, args.ViewModelName);
                }
                catch (Exception)
                {

                    await _messageDialogService.ShowInfoDialogAsync("Could not load the entity, maybe it was deleted in the meantime by another user. Tha navigation is refreshed for you.");
                    await NavigationViewModel.LoadAsync();
                    return;
                }

                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel;
        }

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { Id = nextNewItemId--, ViewModelName = viewModelType.Name });
        }

        private void OnOpenSingleDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { Id = -1, ViewModelName = viewModelType.Name });
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == id
                && vm.GetType().Name == viewModelName);

            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
                _context.UnRegisterOpenedDetailView(detailViewModel, viewModelName);
            }
        }
    }
}
