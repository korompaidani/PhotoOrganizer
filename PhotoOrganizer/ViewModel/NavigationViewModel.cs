using PhotoOrganizer.Data;
using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IPhotoLookupDataService _photoLookupDataService;
        public ObservableCollection<LookupItem> Photos { get; set; }

        public NavigationViewModel(IPhotoLookupDataService photoLookupDataService)
        {
            _photoLookupDataService = photoLookupDataService;
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
