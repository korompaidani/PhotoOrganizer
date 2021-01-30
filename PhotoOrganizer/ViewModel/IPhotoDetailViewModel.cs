using System.Threading.Tasks;

namespace PhotoOrganizer.ViewModel
{
    public interface IPhotoDetailViewModel
    {
        Task LoadAsync(int photoId);
    }
}