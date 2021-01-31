using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoDetailViewModel : ViewModelBase, IPhotoDetailViewModel
    {
        private PhotoWrapper _photo;
        private IPhotoRepository _dataService;
        private IEventAggregator _eventAggregator;

        public ICommand SaveCommand { get; }

        public PhotoWrapper Photo
        { 
            get { return _photo; }
            private set 
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public PhotoDetailViewModel(IPhotoRepository dataService, IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Subscribe(OnOpenFriendDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(int photoId)
        {
            var photo = await _dataService.GetByIdAsync(photoId);
            Photo = new PhotoWrapper(photo);

            Photo.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Photo.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnSaveExecute()
        {
            _dataService.SaveAsync(Photo.Model);
            _eventAggregator.GetEvent<AfterPhotoSavedEvent>().Publish(
                new AfterPhotoSavedEventArgs
                {
                    Id = Photo.Id,
                    Title = $"{Photo.Title}"
                });
        }

        private bool OnSaveCanExecute()
        {
            //Check if Photo in addition if photo has changes

            return Photo != null && !Photo.HasErrors;
        }

        private async void OnOpenFriendDetailView(int photoId)
        {
            await LoadAsync(photoId);
        }

        
    }
}
