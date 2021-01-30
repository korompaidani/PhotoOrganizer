using PhotoOrganizer.Event;
using PhotoOrganizer.Model;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.ViewModel
{
    public class PhotoDetailViewModel : ViewModelBase, IPhotoDetailViewModel
    {
        private Photo _photo;
        private IPhotoDataService _dataService;
        private IEventAggregator _eventAggregator;

        public ICommand SaveCommand { get; }

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

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private void OnSaveExecute()
        {
            _dataService.SaveAsync(Photo);
            _eventAggregator.GetEvent<AfterPhotoSavedEvent>().Publish(
                new AfterPhotoSavedEventArgs
                {
                    Id = Photo.Id,
                    Title = $"{Photo.Title}"
                });
        }

        private bool OnSaveCanExecute()
        {
            //Check if Photo attr is valid

            return true;
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
