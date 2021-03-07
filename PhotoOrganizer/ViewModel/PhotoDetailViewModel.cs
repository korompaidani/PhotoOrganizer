using PhotoOrganizer.Common;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
        private bool _isFinalized = false;

        public ICommand FinalizeCommand { get; }
        public ICommand OpenPhotoCommand { get; }        
        public ICommand OpenMapCommand { get; }
        public ICommand OpenPeopleAddViewCommand { get; }
        public ICommand MarkAsUnchanged { get; }

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
                _date = new DateTime(value.Year, value.Month, value.Day, 12, 00, 00, new CultureInfo("hu-HU", false).Calendar);
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
            ILocationLookupDataService locationLookupDataService
            ) : base(eventAggregator, messageDialogService)
        {
            _photoRepository = photoRepository;
            _locationLookupDataService = locationLookupDataService;
            _date = new DateTime(1986, 05, 02, 12, 00, 00, new CultureInfo("hu-HU", false).Calendar);

            EventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(AfterDetailSaved);
            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Subscribe(AfterCollectionSaved);
            EventAggregator.GetEvent<SetCoordinatesEvent>()
                .Subscribe(AfterSetCoordinatesOnMap);
            EventAggregator.GetEvent<AfterPeopleDeletedEvent>()
                .Subscribe(AfterPeopleDeleted);
            

            OpenPhotoCommand = new DelegateCommand(OnOpenPhoto);
            OpenMapCommand = new DelegateCommand(OnOpenMap);
            OpenPeopleAddViewCommand = new DelegateCommand(OnOpenPeopleAddView);
            FinalizeCommand = new DelegateCommand(OnFinalizeExecute);
            MarkAsUnchanged = new DelegateCommand(OnMarkAsUnchanged);

            Locations = new ObservableCollection<LookupItem>();
            Peoples = new ObservableCollection<PeopleItemViewModel>();            
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

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            if(args.ViewModelName == nameof(MapViewModel))
            {
                await LoadLocationLookupAsync();
                Photo.LocationId = args.Id;
            }
        }

        public override async Task LoadAsync(int photoId)
        {
            var photo = photoId > 0
                ? await _photoRepository.GetByIdAsync(photoId)
                : CreateNewPhoto();

            Id = photoId;

            InitializePhoto(photo);
            InitializePeople(photo.Peoples);
            InitializeDateAndTime(photo);
            await LoadLocationLookupAsync();
        }

        private void InitializeDateAndTime(Photo photo)
        {
            if (photo.Month != 0 && photo.Day != 0)
            {
                TakenDate = new DateTime(photo.Year, photo.Month, photo.Day, 12, 00, 00, new CultureInfo("hu-HU", false).Calendar);
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
            Photo.LocationId = null;
            Photo.Coordinates = args.Coordinates;
        }

        private void OnOpenPhoto()
        {
            EventAggregator.GetEvent<OpenPhotoViewEvent>().
                Publish(
                    new OpenPhotoViewEventArgs
                    {
                        Id = Id,
                        FullPath = Photo.FullPath,
                        ViewModelName = "PhotoViewModel"
                    });
        }

        private void OnOpenMap()
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
                        ViewModelName = "MapViewModel"
                    });
        }

        private async void OnOpenPeopleAddView()
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
            Photo.LocationChanged += InitilizationAfterLocationChanged;
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
                if(e.PropertyName == nameof(Photo.Title))
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Photo.Id == 0)
            {
                Photo.Title = "";
                Photo.FullPath = "";
            }

            SetTitle();
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

        private void InitilizationAfterLocationChanged(object sender, LocationChangedEventArgs args)
        {
            if(Photo.LocationId != null)
            {
                Photo.Coordinates = Locations.FirstOrDefault(l => l.Id == Photo.LocationId).Coordinates;
            }
            else
            {
                Photo.Coordinates = null;
            }
        }

        private void RaiseDetailSavedEvent(int modelId, string title, ColorSign colorFlag)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs
                {
                    Id = modelId,
                    Title = title,
                    ColorFlag = ColorMap.Map[colorFlag],
                    ViewModelName = this.GetType().Name
                });
        }

        protected override async void OnSaveExecute()
        {
            if (!_isFinalized)
            {
                Photo.ColorFlag = ColorSign.Modified;
            }            

            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync, 
                () => 
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                });           
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
                await MessageDialogService.ShowInfoDialogAsync($"{Photo.Title} can't be deleted as it is part of at least one album.");
                return;
            }

            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete {Photo.Title}?", "Question");
            if(result == MessageDialogResult.Ok)
            {
                _photoRepository.Remove(Photo.Model);
                await _photoRepository.SaveAsync();
                RaiseDetailDeletedEvent(Photo.Id);                
            }
        }

        private async void OnFinalizeExecute()
        {
            Photo.ColorFlag = ColorSign.Finalized;
            _isFinalized = true;

            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync,
                () =>
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                });
        }

        private async void OnMarkAsUnchanged()
        {
            Photo.ColorFlag = ColorSign.Unmodified;
            _isFinalized = true;

            await SaveWithOptimisticConcurrencyAsync(_photoRepository.SaveAsync,
                () =>
                {
                    HasChanges = _photoRepository.HasChanges();
                    Id = Photo.Id;
                    RaiseDetailSavedEvent(Photo.Id, $"{Photo.Title}", Photo.ColorFlag);
                });
        }

    }
}
