using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public interface IPhotoDetailViewModel
    {
        bool HasChanges { get; }
        Task LoadAsync(int photoId);        
    }
}