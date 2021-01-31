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
        private IPhotoDetailViewModel _photoDetailViewModel;
        private Func<IPhotoDetailViewModel> _photoDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;        

        public ICommand CreateNewPhotoCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }
        public IPhotoDetailViewModel PhotoDetailViewModel 
        { 
            get 
            {
                return _photoDetailViewModel;
            } 
            private set 
            {
                _photoDetailViewModel = value;
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
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Subscribe(OnOpenFriendDetailView);

            CreateNewPhotoCommand = new DelegateCommand(OnCreateNewPhotoExecute);
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenFriendDetailView(int? photoId)
        {
            if(PhotoDetailViewModel != null && PhotoDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("Are you sure to leave this form? Changes will lost.", "Question");
                if(result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            PhotoDetailViewModel = _photoDetailViewModelCreator();
            await PhotoDetailViewModel.LoadAsync(photoId);
        }

        private void OnCreateNewPhotoExecute()
        {
            OnOpenFriendDetailView(null);
        }
    }
}
