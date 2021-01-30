using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.ViewModel
{
    public class PhotoDetailViewModel : ViewModelBase, IPhotoDetailViewModel
    {
        private Photo _photo;
        private IPhotoDataService _dataService;

        public Photo Photo { get; set; }

        public PhotoDetailViewModel(IPhotoDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task LoadAsync(int photoId)
        {
            Photo = await _dataService.GetByIdAsync(photoId);
        }
    }
}
