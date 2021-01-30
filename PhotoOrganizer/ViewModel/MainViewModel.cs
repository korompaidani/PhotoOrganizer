using PhotoOrganizer.Model;
using System.Collections.ObjectModel;

namespace PhotoOrganizer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IPhotoDataService _photoDataService;
        private Photo _selectedPhoto;

        public ObservableCollection<Photo> Photos { get; set; }

        public Photo SelectedPhoto
        {
            get { return _selectedPhoto; }
            set 
            { 
                _selectedPhoto = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IPhotoDataService photoDataService)
        {
            Photos = new ObservableCollection<Photo>();
            _photoDataService = photoDataService;
        }

        public void Load()
        {
            var photos = _photoDataService.GetAll();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(photo);
            }
        }
    }
}
