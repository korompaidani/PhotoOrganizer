using PhotoOrganizer.Data;
using PhotoOrganizer.Event;
using PhotoOrganizer.Model;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        private IEventAggregator _eventAggregator;
        private NavigationItemViewModel _selectedPhoto;

        public ObservableCollection<NavigationItemViewModel> Photos { get; set; }

        public NavigationItemViewModel SelectedPhoto
        {
            get { return _selectedPhoto; }
            set
            {
                _selectedPhoto = value;
                OnPropertyChanged();
                if(_selectedPhoto != null)
                {
                    _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Publish(_selectedPhoto.Id);
                }
            }
        }

        public NavigationViewModel(IPhotoLookupDataService photoLookupDataService, IEventAggregator eventAggregator)
        {
            _photoLookupDataService = photoLookupDataService;
            _eventAggregator = eventAggregator;
            Photos = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterPhotoSavedEvent>().Subscribe(AfterPhotoSaved);
        }

        private void AfterPhotoSaved(AfterPhotoSavedEventArgs obj)
        {
            var lookupItem = Photos.Single(p => p.Id == obj.Id);
            lookupItem.Title = obj.Title;
        }

        public async Task LoadAsync()
        {
            var photos = await _photoLookupDataService.GetPhotoLookupAsync();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(new NavigationItemViewModel(photo.Id, photo.Title));
            }
        }
    }
}
