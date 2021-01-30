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
        private LookupItem _selectedPhoto;

        public ObservableCollection<LookupItem> Photos { get; set; }

        public LookupItem SelectedPhoto
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
            Photos = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var photos = await _photoLookupDataService.GetPhotoLookupAsync();
            Photos.Clear();
            foreach (var photo in photos)
            {
                Photos.Add(photo);
            }
        }
    }
}
