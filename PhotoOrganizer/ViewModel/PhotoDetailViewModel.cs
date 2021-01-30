using PhotoOrganizer.Event;
using PhotoOrganizer.Model;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer.ViewModel
{
    public class PhotoDetailViewModel : ViewModelBase, IPhotoDetailViewModel
    {
        private Photo _photo;
        private IPhotoDataService _dataService;
        private IEventAggregator _eventAggregator;

        public Photo Photo
        { 
            get { return _photo; }
            private set 
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public PhotoDetailViewModel(IPhotoDataService dataService, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Subscribe(OnOpenFriendDetailView);
        }

        private async void OnOpenFriendDetailView(int photoId)
        {
            await LoadAsync(photoId);
        }

        public async Task LoadAsync(int photoId)
        {
            Photo = await _dataService.GetByIdAsync(photoId);
        }
    }
}
