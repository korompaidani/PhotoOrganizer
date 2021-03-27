using Autofac;
using Autofac.Features.Indexed;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Helpers;
using PhotoOrganizer.UI.Resources.Language;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        public ICommand AddNewPhotoCommand { get; }
        public ICommand OpenSingleDetailViewCommand { get; }
        public ICommand OpenSettingsViewCommand { get; }
        public ICommand CreatePhotosFromLibraryCommand { get; }
        public ICommand DeleteDatabaseCommand { get; }
        public ICommand CreateDatabaseBackupCommand { get; }
        public ICommand WriteAllSavedMetadataCommand { get; }
        public ICommand CloseOpenTabsCommand { get; }
        public ICommand SaveAllOpenTab { get; }
        public ICommand ExitCommand { get; }

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
            AddNewPhotoCommand = new DelegateCommand(OnAddNewPhotoExecute);
            CreatePhotosFromLibraryCommand = new DelegateCommand(OnCreatePhotosFromLibraryExecute);
            DeleteDatabaseCommand = new DelegateCommand(OnDeleteDatabaseExecute);
            CreateDatabaseBackupCommand = new DelegateCommand(OnCreateDatabaseBackupExecute);
            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailExecute);
            OpenSettingsViewCommand = new DelegateCommand(OnOpenSettingsViewExecute);
            WriteAllSavedMetadataCommand = new DelegateCommand(OnWriteAllSavedMetadataCommand);
            CloseOpenTabsCommand = new DelegateCommand(OnCloseOpenTabs);
            SaveAllOpenTab = new DelegateCommand(OnSaveAllOpenTab);
            ExitCommand = new DelegateCommand(OnExitExecute);
        }

        private async void OnExitExecute()
        {
            await _context.SaveAllOpenedDetailView();
        }

        private async void OnSaveAllOpenTab()
        {
            var result = await _context.SaveAllTab(isForceSaveAll: true);

            _eventAggregator.GetEvent<AfterTabClosedEvent>().
                Publish(
                    new AfterTabClosedEventArgs
                    {
                        DetailInfo = result.Select(d => d.Value).ToList()
                    });
        }

        private async void OnCloseOpenTabs()
        {
            var result = await CloseAllTabsAsync();

            _eventAggregator.GetEvent<AfterTabClosedEvent>().
                Publish(
                    new AfterTabClosedEventArgs
                    {
                        DetailInfo = result.Select(d => d.Value).ToList()
                    });
        }

        private async void AfterProgressWindowClosed(CloseProgressWindowEventArgs args)
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnWriteAllSavedMetadataCommand()
        {
            await CloseAllTabsAsync();
            await _messageDialogService.ShowProgressDuringTaskAsync(TextResources.PleaseWait_windowTitle, TextResources.WritingMetadata_message, _photoMetaWrapperService.WriteMetaInfoToAllFileAsync);
            await NavigationViewModel.LoadAsync();

            // Display error messages if it is any
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
            await CloseAllTabsAsync(true);
            await _directoryReaderWrapperService.LoadAllFromLibraryAsync();
            await LoadAsync();
        }

        private async void OnCreateDatabaseBackupExecute()
        {
            await _directoryReaderWrapperService.CreateBackup();
        }

        private async void OnDeleteDatabaseExecute()
        {
            await CloseAllTabsAsync(true);
            await _directoryReaderWrapperService.EraseFormerData();
            await LoadAsync();
        }

        private async Task<List<KeyValuePair<int, PhotoDetailInfo>>> CloseAllTabsAsync(bool isForceSave = false)
        {
            var result = await _context.SaveAllTab(isForceSave);

            foreach (var tab in result)
            {
                RemoveDetailViewModel(tab.Key, tab.Value.ViewModelName);
            }

            return result;
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

                    await _messageDialogService.ShowInfoDialogAsync(TextResources.CouldNotLoadEntity_message);
                    await NavigationViewModel.LoadAsync();
                    return;
                }

                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel;
        }

        private async void OnAddNewPhotoExecute()
        {
            var id = await _directoryReaderWrapperService.LoadSinglePhotoFromLibraryAsync();
            OnOpenDetailView(new OpenDetailViewEventArgs { Id = id, ViewModelName = nameof(PhotoDetailViewModel) });
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
