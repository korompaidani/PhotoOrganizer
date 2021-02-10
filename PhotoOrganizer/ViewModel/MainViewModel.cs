using Autofac.Features.Indexed;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IDetailViewModel _selectedDetailViewModel;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private int nextNewItemId = 0;

        public ICommand CreateNewDetailCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }

        private IIndex<string, IDetailViewModel> _detailViewModelCreator;

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

        public MainViewModel(INavigationViewModel navigationViewModel, 
            IIndex<string, IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            NavigationViewModel = navigationViewModel;

            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().
                Subscribe(AfterDetailDeleted);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().
                Subscribe(AfterDetailClosed);

            DetailViewModels = new ObservableCollection<IDetailViewModel>();
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id
                && vm.GetType().Name == args.ViewModelName);

            if(detailViewModel == null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                await detailViewModel.LoadAsync(args.Id);
                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel;
        }
        
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { Id = nextNewItemId--, ViewModelName = viewModelType.Name });
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
            }
        }
    }
}
