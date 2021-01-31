using PhotoOrganizer.Model;
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
        private IPhotoRepository _photoRepository;
        private IEventAggregator _eventAggregator;
        private bool _hasChanges;

        public ICommand SaveCommand { get; }

        public PhotoWrapper Photo
        { 
            get { return _photo; }
            set 
            {
                _photo = value;
                OnPropertyChanged();                
            }
        }
        
        public bool HasChanges
        {
            get { return _hasChanges; }
            private set 
            { 
                if(_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public PhotoDetailViewModel(IPhotoRepository photoRepository, IEventAggregator eventAggregator)
        {
            _photoRepository = photoRepository;
            _eventAggregator = eventAggregator;
            
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(int? photoId)
        {
            var photo = photoId.HasValue
                ? await _photoRepository.GetByIdAsync(photoId.Value)
                : CreateNewPhoto();
            
            Photo = new PhotoWrapper(photo);
            Photo.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _photoRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Photo.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if(Photo.Id == 0)
            {
                Photo.Title = "";
                Photo.FullPath = "";
            }
        }

        private Photo CreateNewPhoto()
        {
            var photo = new Photo();
            _photoRepository.Add(photo);
            return photo;
        }

        private async void OnSaveExecute()
        {
            await _photoRepository.SaveAsync();
            HasChanges = _photoRepository.HasChanges();
            _eventAggregator.GetEvent<AfterPhotoSavedEvent>().Publish(
                new AfterPhotoSavedEventArgs
                {
                    Id = Photo.Id,
                    Title = $"{Photo.Title}"
                });
        }

        private bool OnSaveCanExecute()
        {
            return Photo != null && !Photo.HasErrors && HasChanges;
        }        
    }
}
