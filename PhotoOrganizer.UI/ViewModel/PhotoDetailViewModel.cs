using PhotoOrganizer.Common;
using PhotoOrganizer.MapTools;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Resources.Language;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.View;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoDetailViewModel : DetailViewModelBase, IPhotoDetailViewModel
    {
        private PhotoWrapper _photo;

        private ILocationLookupDataService _locationLookupDataService;
        private IPhotoRepository _photoRepository;
        private DateTime _date;
        private IBulkAttributeSetterService _bulkAttributeSetter;
        private IPhotoMetaWrapperService _photoMetaWrapperService;
        private bool _isDetailViewInitialized = false;
        private bool _evenJustFinalizedOrFromOtherReason = false;
        private bool _isAnySelectedNavigationItem = false;
        private bool _isPhotoOnShelve;

        public ICommand FinalizeCommand { get; }
        public ICommand OpenPhotoCommand { get; }        
        public ICommand OpenMapCommand { get; }
        public ICommand OpenPeopleAddViewCommand { get; }
        public ICommand MarkAsUnchanged { get; }
        public ICommand BulkSetAttribute { get; }
        public ICommand AddToShelveCommand { get; }
        public ICommand RemoveFromShelveCommand { get; }
        public ICommand WriteMetadataCommand { get; }
        public ICommand CopyToClipBoardCommand { get; }
        public ICommand OpenCoordinatesInBrowserCommand { get; }

        public ObservableCollection<LookupItem> Locations { get; }
        public ObservableCollection<PeopleItemViewModel> Peoples { get; }

        public PhotoWrapper Photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public DateTime TakenDate
        {
            get { return _date; }
            set
            {
                Photo.Year = value.Year;
                Photo.Month = value.Month;
                Photo.Day = value.Day;
                _date = new DateTime(value.Year, value.Month, value.Day, new CultureInfo(TextResources.CultureInfo_constant, false).Calendar);
                OnPropertyChanged();
            }
        }

        public DateTime TakenTime
        {
            get { return new DateTime(Photo.HHMMSS.Ticks); }
            set
            {
                Photo.HHMMSS = new TimeSpan(value.Hour, value.Minute, value.Second);
                OnPropertyChanged();
            }
        }

        public PhotoDetailViewModel(
            IPhotoRepository photoRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            ILocationLookupDataService locationLookupDataService,
            IBulkAttributeSetterService bulkAttributeSetter,
            IPhotoMetaWrapperService photoMetaWrapperService
            ) : base(eventAggregator, messageDialogService)
        {
            _photoRepository = photoRepository;
            _locationLookupDataService = locationLookupDataService;            
            _bulkAttributeSetter = bulkAttributeSetter;
            _photoMetaWrapperService = photoMetaWrapperService;

            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Subscribe(AfterCollectionSaved);
            EventAggregator.GetEvent<AfterPeopleDeletedEvent>()
                .Subscribe(AfterPeopleDeleted);
            EventAggregator.GetEvent<SetCoordinatesEvent>()
                .Subscribe(AfterSetCoordinatesOnMap);
            EventAggregator.GetEvent<SaveCoordinatesAsNewEvent>()
                .Subscribe(AfterSaveCoordinatesAsNew);
            EventAggregator.GetEvent<SaveCoordinatesAsOverrideEvent>()
                .Subscribe(AfterSaveCoordinatesAsOverride);
            EventAggregator.GetEvent<SelectionChangedEvent>()
                .Subscribe(AfterSelectionChanged);

            OpenPhotoCommand = new DelegateCommand(OnOpenPhotoExecute);
            OpenMapCommand = new DelegateCommand(OnOpenMapExecute);
            OpenPeopleAddViewCommand = new DelegateCommand(OnOpenPeopleAddViewExecute);
            FinalizeCommand = new DelegateCommand(OnFinalizeExecute);
            MarkAsUnchanged = new DelegateCommand(OnMarkAsUnchangedExecute);
            BulkSetAttribute = new DelegateCommand<string>(OnBulkSetAttributeExecute, OnBulkSetAttributeCanExecute);
            AddToShelveCommand = new DelegateCommand(OnAddToShelveExecute, OnAddToShelveCanExecute);
            RemoveFromShelveCommand = new DelegateCommand(OnRemoveFromShelveExecute, OnRemoveFromShelveCanExecute);
            WriteMetadataCommand = new DelegateCommand(OnWriteMetadataExecute, OnWriteMetadataCanExecute);
            CopyToClipBoardCommand = new DelegateCommand(OnCopyToClipBoardExecute);
            OpenCoordinatesInBrowserCommand = new DelegateCommand(OnOpenCoordinatesInBrowserExecute);

            Locations = new ObservableCollection<LookupItem>();
            Peoples = new ObservableCollection<PeopleItemViewModel>();            
        }

        private void OnOpenCoordinatesInBrowserExecute()
        {
            var googleMapsLink = Photo.Coordinates.ConvertCoordinateToGoogleMapUrl();
            Process.Start(googleMapsLink);
        }

        private void OnCopyToClipBoardExecute()
        {
            Clipboard.SetText(Photo.FullPath);
        }

        private bool OnWriteMetadataCanExecute()
        {
            if(Photo.ColorFlag == ColorSign.Finalized)
            {
                return false;
            }
            return true;            
        }

        private async void OnWriteMetadataExecute()
        {
            var result = _photoMetaWrapperService.WriteMetaInfoToSingleFile(Photo.Model, Photo.FullPath);
            var message = result ? TextResources.FileHasBeenModifiedSucc_message : TextResources.FileHasBeenModifiedFailed_message;
            if (result)
            {
                SetFinalizedStateFlag();
                await SaveChanges(false, true);
            }

            await MessageDialogService.ShowInfoDialogAsync(message);
        }

        private bool OnRemoveFromShelveCanExecute()
        {
            return !HasChanges && _isPhotoOnShelve;
        }

        private async void OnRemoveFromShelveExecute()
        {
            await _photoRepository.RemovePhotoToShelveAsync(Photo.Model);
            _isPhotoOnShelve = false;
            ((DelegateCommand)AddToShelveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveFromShelveCommand).RaiseCanExecuteChanged();

            RaiseDetailSavedEvent(Photo.Id, Photo.Title, Photo.ColorFlag, true, true);
        }

        private bool OnAddToShelveCanExecute()
        {
            return !HasChanges && !_isPhotoOnShelve;
        }

        private async void OnAddToShelveExecute()
        {
            await _photoRepository.AddPhotoToShelveAsync(Photo.Model);
            _isPhotoOnShelve = true;
            ((DelegateCommand)AddToShelveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveFromShelveCommand).RaiseCanExecuteChanged();

            RaiseDetailSavedEvent(Photo.Id, Photo.Title, Photo.ColorFlag, true, false);
        }

        private async void AfterSaveCoordinatesAsOverride(SaveCoordinatesAsOverrideEventArgs args)
        {
            await LoadLocationLookupAsync();
            if (args.PhotoId == Photo.Id)
            {
                Photo.Coordinates = args.Coordinates;
                Photo.LocationId = args.LocationId;
            }
        }

        private async void AfterSaveCoordinatesAsNew(SaveCoordinatesAsNewEventArgs args)
        {
            await LoadLocationLookupAsync();
            if (args.PhotoId == Photo.Id)
            {
                Photo.Coordinates = args.Coordinates;
                Photo.LocationId = args.LocationId;
            }
        }

        private void AfterPeopleDeleted(AfterPeopleDeletedEventArgs args)
        {
            var peopleItem = Peoples.FirstOrDefault(p => p.Id == args.Id);
            if(peopleItem != null)
            {
                Peoples.Remove(peopleItem);
                Photo.RemovePeople(peopleItem.People.Model);
            }
        }

        public override async Task LoadAsync(int photoId)
        {
            var photo = photoId > 0
                ? await _photoRepository.GetByIdAsync(photoId)
                : CreateNewPhoto();

            Id = photoId;

            _isPhotoOnShelve = await Task<bool>.Run(() => _photoRepository.IsPhotoExistOnShelve(Id));

            InitializePhoto(photo);
            InitializePeople(photo.Peoples);
            InitializeDateAndTime(photo);
            RaiseOpenPhotoDetailView();
            await LoadLocationLookupAsync();
            ((DelegateCommand<string>)BulkSetAttribute).RaiseCanExecuteChanged();
        }

        private void RaiseOpenPhotoDetailView()
        {
            if (!_isDetailViewInitialized)
            {
                EventAggregator.GetEvent<OpenPhotoDetailViewEvent>().
                Publish(
                    new OpenPhotoDetailViewEventArgs
                    {
                        Id = Id,
                        ViewModelName = this.GetType().Name,
                        DetailView = this
                    });

                _isDetailViewInitialized = true;
            }
        }

        private void InitializeDateAndTime(Photo photo)
        {
            if (photo.Month != 0 && photo.Day != 0)
            {
                TakenDate = new DateTime(photo.Year, photo.Month, photo.Day, photo.HHMMSS.Hours, photo.HHMMSS.Minutes, photo.HHMMSS.Seconds, new CultureInfo(TextResources.CultureInfo_constant, false).Calendar);
            }
            TakenTime = new DateTime(photo.HHMMSS.Ticks);
        }

        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if(args.ViewModelName == nameof(LocationDetailViewModel))
            {
                await LoadLocationLookupAsync();
            }
            if(args.ViewModelName == nameof(PeopleItemViewModel))
            {

            }
        }

        private void AfterSetCoordinatesOnMap(SetCoordinatesEventArgs args)
        {
            if(args.PhotoId == Photo.Id)
            {
                Photo.LocationId = null;
                Photo.Coordinates = args.Coordinates;
            }
        }

        private void OnOpenPhotoExecute()
        {
            EventAggregator.GetEvent<OpenPhotoViewEvent>().
                Publish(
                    new OpenPhotoViewEventArgs
                    {
                        Id = Id,
                        FullPath = Photo.FullPath,
                        ViewModelName = nameof(PhotoViewModel)
                    });
        }

        private void OnOpenMapExecute()
        {
            int locationId = 0;
            if(Photo.LocationId != null)
            {
                locationId = (int)Photo.LocationId;
            }

            EventAggregator.GetEvent<OpenMapViewEvent>().
                Publish(
                    new OpenMapViewEventArgs
                    {
                        Id = locationId,
                        PhotoId = Id,
                        Coordinates = Photo.Coordinates, //nullcheck on the other side
                        ViewModelName = nameof(MapViewModel)
                    });
        }

        private async void OnOpenPeopleAddViewExecute()
        {
            var window = new PeopleSelectionCreationView();
            var peopleSelectionViewModel = new PeopleSelectionCreationViewModel(_photoRepository, Peoples, this, EventAggregator);
            await peopleSelectionViewModel.LoadAsync();

            window.DataContext = peopleSelectionViewModel;
            window.Owner = Application.Current.MainWindow;

            window.ShowDialog();
        }

        private void InitializePeople(ICollection<People> peoples)
        {
            foreach(var people in Peoples)
            {
                people.PropertyChanged -= PeopleWrapper_PropertyChanged;
            }
            Peoples.Clear();
            foreach(var people in peoples)
            {
                var wrapper = new PeopleWrapper(people);
                var peopleItem = new PeopleItemViewModel(wrapper, EventAggregator);
                Peoples.Add(peopleItem);
                wrapper.PropertyChanged += PeopleWrapper_PropertyChanged;
            }
        }

        public void PeopleWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _photoRepository.HasChanges();
            }
            if(e.PropertyName == nameof(PeopleWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void InitializePhoto(Photo photo)
        {       
            Photo = new PhotoWrapper(photo);
            Photo.PropertyChanged += (s, e) =>
            {
                ((DelegateCommand)AddToShelveCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveFromShelveCommand).RaiseCanExecuteChanged();
                if (!HasChanges)
                {
                    HasChanges = _photoRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Photo.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if(e.PropertyName == nameof(Photo.Title))
                {
                    SetTitle();
                }
                if (e.PropertyName == nameof(Photo.LocationId))
                {
                    SetCoordinateBasedOnLookupChange();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Photo.Id == 0)
            {
                Photo.Title = "";
                Photo.FullPath = "";
            }

            SetTitle();
            if(photo.Year < 2)
            {
                _date = new DateTime(1986, 05, 02, 12, 00, 00, new CultureInfo(TextResources.CultureInfo_constant, false).Calendar);
            }
        }

        private void SetCoordinateBasedOnLookupChange()
        {
            if (Locations != null)
            {
                if (Photo.LocationId == null)
                {
                    if(Photo.Coordinates != null)
                    {
                        Photo.Coordinates = null;
                    }
                    return;
                }
                var coordinates = Locations.FirstOrDefault(l => l.Id == Photo.LocationId);
                if (coordinates != null)
                {
                    if(Photo.Coordinates != coordinates.Coordinates)
                    {
                        Photo.Coordinates = coordinates.Coordinates;
                    }
                }
            }
        }

        private void SetTitle()
        {
            Title = $"{Photo.Title}";
        }

        private async Task LoadLocationLookupAsync()
        {
            Locations.Clear();
            Locations.Add(new NullLookupItem { DisplayMemberItem = "-" });
            var lookup = await _locationLookupDataService.GetLocationLookupAsync();
            foreach (var lookupItem in lookup)
            {
                Locations.Add(lookupItem);
            }
        }

        // TODO: refactor if PhotoService layer has been created this should be moved there
        private Photo CreateNewPhoto()
        {
            var photo = new Photo();
            _photoRepository.Add(photo);
            return photo;
        }

        private void RaiseDetailSavedEvent(int modelId, string title, ColorSign colorFlag, bool isShelveChange = false, bool isRemovingFromShelve = false)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs
                {
                    Id = modelId,
                    Title = title,
                    ColorFlag = ColorMap.Map[colorFlag],
                    ViewModelName = this.GetType().Name,
                    IsShelveChanges = isShelveChange,
                    IsRemovingFromShelve = isRemovingFromShelve,
                    PhotoPath = Photo.FullPath
                });
        }

        public override async Task SaveChanges(bool isClosing, bool isOptimistic)
        {
            await Save(isClosing, isOptimistic);
        }

        protected override async void OnSaveExecute()
        {
            await Save(false, true);
        }

        protected override bool OnSaveCanExecute()
        {
            return Photo != null 
                && !Photo.HasErrors 
                && Peoples.All(p => !p.People.HasErrors)
                && HasChanges;
        }

        protected override async void OnDeleteExecute()
        {
            if(await _photoRepository.HasAlbums(Photo.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync(String.Format(TextResources.PhotoCantDeleted_message, Photo.Title));
                return;
            }

            var result = await MessageDialogService.ShowOkCancelDialogAsync(String.Format(TextResources.DoYouReallyWantDelete_message, Photo.Title), TextResources.Question_windowTitle);
            if(result == MessageDialogResult.Ok)
            {
                _photoRepository.Remove(Photo.Model);
                await _photoRepository.SaveAsync();
                RaiseDetailDeletedEvent(Photo.Id);                
            }
        }

        private async Task Save(bool isClosing, bool isOptimistic)
        {
            if (!_evenJustFinalizedOrFromOtherReason)
            {
                SetModifiedFlag();                
            }
            _evenJustFinalizedOrFromOtherReason = false;

            if (isOptimistic)
            {
                await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync,
                () =>
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    if (!isClosing)
                    {
                        RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                    }
                });
            }
            else
            {
                await SaveWithPessimisticConcurrencyAsync(_photoRepository.SaveAsync,
                () =>
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    if (!isClosing)
                    {
                        RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                    }
                });
            }

            if (!isClosing)
            {
                ((DelegateCommand)AddToShelveCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveFromShelveCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)WriteMetadataCommand).RaiseCanExecuteChanged();
            }
        }

        public void SetModifiedFlag()
        {
            Photo.ColorFlag = ColorSign.Modified;
        }

        private void SetFinalizedStateFlag()
        {
            Photo.ColorFlag = ColorSign.Finalized;
            _evenJustFinalizedOrFromOtherReason = true;
        }

        private void SetUnmodifiedStateFlag()
        {
            Photo.ColorFlag = ColorSign.Unmodified;
            _evenJustFinalizedOrFromOtherReason = false;
        }

        private async void OnFinalizeExecute()
        {
            SetFinalizedStateFlag();

            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync,
                () =>
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                });

            ((DelegateCommand)WriteMetadataCommand).RaiseCanExecuteChanged();
        }

        private async void OnMarkAsUnchangedExecute()
        {
            SetUnmodifiedStateFlag();

            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync,
                () =>
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                });

            ((DelegateCommand)WriteMetadataCommand).RaiseCanExecuteChanged();
        }

        private void AfterSelectionChanged(SelectionChangedEventArgs args)
        {
            _isAnySelectedNavigationItem = args.IsAnySelectedItem;
            ((DelegateCommand<string>)BulkSetAttribute).RaiseCanExecuteChanged();
        }

        private bool OnBulkSetAttributeCanExecute(string propertyName)
        {
            if (_bulkAttributeSetter.IsAnySelectedItem(Photo.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }      

        private async void OnBulkSetAttributeExecute(string propertyName)
        {
            var propertyNamesAndValues = new Dictionary<string, object>();
            
            {
                List<string> propertyNames = FilterIfMoreProperties(propertyName);
                foreach (var propName in propertyNames)
                {
                    PropertyInfo prop = Photo.Model.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null)
                    {
                        propertyNamesAndValues.Add(prop.Name, prop.GetValue(Photo.Model));
                    }
                }
            }

            _evenJustFinalizedOrFromOtherReason = true;
            await SaveChanges(false, true);

            EventAggregator.GetEvent<BulkSetPhotoDetailAttributesEvent>().Publish(
                new BulkSetPhotoDetailAttributesEventArgs
                {
                    PropertyNamesAndValues = propertyNamesAndValues,
                    CallerId = Id,
                    PhotoRepository = _photoRepository
                });
        }

        private List<string> FilterIfMoreProperties(string propertyName)
        {
            var resultList = new List<string>();
            if (propertyName.Contains(";"))
            {
                resultList.AddRange(propertyName.Split(';'));
            }
            else
            {
                resultList.Add(propertyName);
            }

            return resultList;
        }

        public override void DisposeConnection()
        {
            _photoRepository.DisposeConnection();
        }
    }
}
