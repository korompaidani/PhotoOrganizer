using PhotoOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Threading.Tasks;

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
            PhotoDetailViewModel = _photoDetailViewModelCreator();
            await PhotoDetailViewModel.LoadAsync(photoId);
        }
    }
}
