using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        public async Task LoadAsync()
        {
            var photos = await _photoDataService.GetAllAsync();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(photo);
            }
        }
    }
}
