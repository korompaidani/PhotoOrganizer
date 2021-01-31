using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public interface IPhotoDetailViewModel
    {
        Task LoadAsync(int photoId);
    }
}