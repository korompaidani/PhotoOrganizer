using PhotoOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IPhotoDetailViewModel _photoDetailViewModel;
        private Func<IPhotoDetailViewModel> _photoDetailViewModelCreator;
        private IEventAggregator _eventAggregator;        

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
            IEventAggregator eventAggregator)
        {
            NavigationViewModel = navigationViewModel;
            _photoDetailViewModelCreator = photoDetailViewModelCreator;
            
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Subscribe(OnOpenFriendDetailView);
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenFriendDetailView(int photoId)
        {
            if(PhotoDetailViewModel != null && PhotoDetailViewModel.HasChanges)
            {
               var result =  MessageBox.Show("Are you sure to leave this form? Changes will lost.", "Question",
                   MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            PhotoDetailViewModel = _photoDetailViewModelCreator();
            await PhotoDetailViewModel.LoadAsync(photoId);
        }
    }
}
