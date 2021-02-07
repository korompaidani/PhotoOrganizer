using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IDetailViewModel _detailViewModel;
        private Func<IPhotoDetailViewModel> _photoDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;        

        public ICommand CreateNewDetailCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }
        public IDetailViewModel DetailViewModel 
        { 
            get 
            {
                return _detailViewModel;
            } 
            private set 
            {
                _detailViewModel = value;
                OnPropertyChanged();
            } 
        }

        public MainViewModel(INavigationViewModel navigationViewModel, 
            Func<IPhotoDetailViewModel> photoDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            NavigationViewModel = navigationViewModel;
            _photoDetailViewModelCreator = photoDetailViewModelCreator;
            _messageDialogService = messageDialogService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().
                Subscribe(AfterDetailDeleted);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }        

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if(DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("Are you sure to leave this form? Changes will lost.", "Question");
                if(result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            switch (args.ViewModelName)
            {
                case nameof(PhotoDetailViewModel):
                    DetailViewModel = _photoDetailViewModelCreator();
                    break;
            }

            await DetailViewModel.LoadAsync(args.Id);
        }

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { ViewModelName = viewModelType.Name });
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        }
    }
}
