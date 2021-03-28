using PhotoOrganizer.Common;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class BulkAttributeSetterService : IBulkAttributeSetterService
    {
        private IEventAggregator _eventAggregator;
        private IPhotoRepository _photoRepository;

        private IDictionary<int, bool> _navigationItemCheckStatusCollection = new Dictionary<int, bool>();
        private HashSet<int> _previousNavigationItemCheckStatusCollection = new HashSet<int>();
        private IDictionary<int, IDetailViewModel> _openedPhotoDetailViews = new Dictionary<int, IDetailViewModel>();

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
            int beforeCount = _navigationItemCheckStatusCollection.Count;
            if (checkStatus == true)
            {
                _navigationItemCheckStatusCollection[photoNavigationLookupId] = checkStatus;
            }
            else
            {
                _navigationItemCheckStatusCollection.Remove(photoNavigationLookupId);
            }
            int afterCount = _navigationItemCheckStatusCollection.Count;
            EvaluateCheckCounts(beforeCount, afterCount);
        }

        public bool HasPreviousSelectionEntries()
        {
            return _previousNavigationItemCheckStatusCollection.Count > 0;
        }

        public HashSet<int> GetPreviousSelection()
        {
            return _previousNavigationItemCheckStatusCollection;
        }

        private void EvaluateCheckCounts(int beforeCount, int afterCount)
        {
            if (afterCount > 0 && afterCount != beforeCount)
            {
                RaiseSelectionChangedEvent(isAnySelected: true);
                return;
            }
            if (afterCount == 0)
            {
                RaiseSelectionChangedEvent(isAnySelected: false);
                return;
            }
        }

        private async void OnBulkSetPhotoDetailAttributes(BulkSetPhotoDetailAttributesEventArgs args)
        {
            _photoRepository = args.PhotoRepository;
            CloseAllOpenDetailViews(args.CallerId);

            await SetPropertiesOfCheckedItems(args.PropertyNamesAndValues, args.CallerId);
            await ReloadNavigation();
            //_photoRepository.DisposeConnection();
        }

        private void CloseAllOpenDetailViews(int callerId)
        {

            foreach (var item in _navigationItemCheckStatusCollection)
            {
                if (item.Key == callerId)
                {
                    continue;
                }

                if (item.Value)
                {
                    if (_openedPhotoDetailViews.ContainsKey(item.Key))
                    {
                        IDetailViewModel detailView;
                        _openedPhotoDetailViews.TryGetValue(item.Key, out detailView);
                        var photoDetailView = detailView as PhotoDetailViewModel;
                        if (photoDetailView != null)
                        {
                            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                                .Publish(new AfterDetailClosedEventArgs
                                {
                                    Id = photoDetailView.Id,
                                    ViewModelName = photoDetailView.GetType().Name
                                });
                        }
                    }
                }
            }
        }

        private void OnOpenPhotoDetailViewEvent(OpenPhotoDetailViewEventArgs args)
        {
            if (args.ViewModelName == nameof(PhotoDetailViewModel))
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
            try 
            {
                foreach (var item in _navigationItemCheckStatusCollection)
                {
                    if (item.Key == callerId)
                    {
                        continue;
                    }

                    if (item.Value)
                    {
                        var photo = await _photoRepository.GetByIdAsync(item.Key);
                        photos.Add(photo);
                        photo.ColorFlag = ColorSign.Modified;
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
            catch
            {

            }
        }

        private async Task ReloadNavigation()
        {
            _previousNavigationItemCheckStatusCollection.Clear();
            var forNavigationItems = new List<Tuple<int, string, string, string>>();
            foreach (var item in _navigationItemCheckStatusCollection)
            {
                var photo = await _photoRepository.GetByIdAsync(item.Key);
                forNavigationItems.Add(new Tuple<int, string, string, string>(photo.Id, photo.Title, ColorMap.Map[photo.ColorFlag], photo.FullPath));

                _previousNavigationItemCheckStatusCollection.Add(item.Key);
            }

            _eventAggregator.GetEvent<AfterBulkSetPhotoDetailAttributesEvent>().
            Publish(
                new AfterBulkSetPhotoDetailAttributesEventArgs
                {
                    NavigationAttributes = forNavigationItems
                });

            _navigationItemCheckStatusCollection.Clear();
        }

        private void RaiseSelectionChangedEvent(bool isAnySelected)
        {
            _eventAggregator.GetEvent<SelectionChangedEvent>().
            Publish(
                new SelectionChangedEventArgs
                {
                    IsAnySelectedItem = isAnySelected
                });
        }

        public bool IsAnySelectedItem(int? exceptId = null)
        {
            if (_navigationItemCheckStatusCollection.Count > 0)
            {
                if (exceptId != null)
                {
                    return !_navigationItemCheckStatusCollection.ContainsKey((int)exceptId);
                }

                return true;
            }

            return false;
        }
    }
}