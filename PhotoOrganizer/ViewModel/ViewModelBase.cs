using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoOrganizer.UI.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected IPhotoRepository _photoRepository;
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected Photo CreateNewPhoto(Photo photo = null)
        {
            if(photo == null)
            {
                photo = new Photo();
            }
            
            _photoRepository.Add(photo);
            return photo;
        }
    }
}
