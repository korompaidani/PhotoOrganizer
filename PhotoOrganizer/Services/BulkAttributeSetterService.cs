using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class BulkAttributeSetterService : IBulkAttributeSetterService
    {
        private IEventAggregator _eventAggregator;
        private IPhotoRepository _photoRepository;

        IDictionary<int, bool> _navigationItemCheckStatusCollection = new Dictionary<int, bool>();
        IDictionary<int, IDetailViewModel> _openedPhotoDetailViews = new Dictionary<int, IDetailViewModel>();

        public BulkAttributeSetterService(
            IEventAggregator eventAggregator,
            IPhotoRepository photoRepository)
        {
            _eventAggregator = eventAggregator;
            _photoRepository = photoRepository;

            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>()
                .Subscribe(OnOpenPhotoDetailViewEvent);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Subscribe(OnClosePhotoDetailClosedEvent);
            _eventAggregator.GetEvent<BulkSetPhotoDetailAttributesEvent>()
                .Subscribe(OnBulkSetPhotoDetailAttributes);
        }

        public bool IsCheckedById(int photoNavigationLookupId)
        {
            bool result = false;
            _navigationItemCheckStatusCollection.TryGetValue(photoNavigationLookupId, out result);
            return result;
        }

        public void SetCheckedStateForId(int photoNavigationLookupId, bool checkStatus)
        {
            if(checkStatus == true)
            {
                _navigationItemCheckStatusCollection[photoNavigationLookupId] = checkStatus;
            }
            else
            {
                _navigationItemCheckStatusCollection.Remove(photoNavigationLookupId);
            }
        }

        private async void OnBulkSetPhotoDetailAttributes(BulkSetPhotoDetailAttributesEventArgs args)
        {
            await SetPropertiesOfCheckedItems(args.PropertyNamesAndValues, args.CallerId);
            await ReloadOpenedPhotoDetailViews();
            //Reload Navigation
            RaiseUncheckPhotoNavigationItemsEvent(); 
        }

        private void OnOpenPhotoDetailViewEvent(OpenPhotoDetailViewEventArgs args)
        {
            if(args.ViewModelName == nameof(PhotoDetailViewModel))
            {
                _openedPhotoDetailViews[args.Id] = args.DetailView;
            }
        }

        private void OnClosePhotoDetailClosedEvent(AfterDetailClosedEventArgs args)
        {
            if (args.ViewModelName == nameof(PhotoDetailViewModel))
            {
                _openedPhotoDetailViews.Remove(args.Id);
            }
        }

        private async Task SetPropertiesOfCheckedItems(IDictionary<string, object> properyNamesAndValues, int callerId)
        {
            List<Photo> photos = new List<Photo>();
            foreach (var item in _navigationItemCheckStatusCollection)
            {
                if(item.Key == callerId)
                {
                    continue;
                }

                if (item.Value)
                {
                    var photo = await _photoRepository.GetByIdAsync(item.Key);
                    photos.Add(photo);

                    foreach (var property in properyNamesAndValues)
                    {
                        PropertyInfo prop = photo.GetType().GetProperty(property.Key, BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null && prop.CanWrite)
                        {
                            prop.SetValue(photo, property.Value, null);
                        }
                    }
                }
            }
            await _photoRepository.SaveAsync();
        }

        private async Task ReloadOpenedPhotoDetailViews()
        {
            foreach (var item in _openedPhotoDetailViews)
            {
                if (item.Value != null)
                {
                    await item.Value.LoadAsync(item.Key);
                }
            }
        }

        private void RaiseUncheckPhotoNavigationItemsEvent()
        {
            var checkedItems = new HashSet<int>();
            foreach (var item in _navigationItemCheckStatusCollection)
            {
                checkedItems.Add(item.Key);
            }

            _eventAggregator.GetEvent<UncheckPhotoNavigationItemsEvent>().
            Publish(
                new UncheckPhotoNavigationItemsEventArgs
                {
                    Ids = checkedItems
                });
        }
    }
}