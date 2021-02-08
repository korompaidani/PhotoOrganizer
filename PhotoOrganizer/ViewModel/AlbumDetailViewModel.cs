using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public class AlbumDetailViewModel : DetailViewModelBase, IAlbumDetailViewModel
    {
        private IAlbumRepository _albumRepository;
        private AlbumWrapper _selectedAlbum;
        private IMessageDialogService _messageDialogService;

        public AlbumWrapper Album
        {
            get { return _selectedAlbum; }
            set
            {
                _selectedAlbum = value;
                OnPropertyChanged();
            }
        }

        public AlbumDetailViewModel(IAlbumRepository albumRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService            
            ) : base(eventAggregator)
        {
            _albumRepository = albumRepository;
            _messageDialogService = messageDialogService;           
        }

        public async override Task LoadAsync(int? id)
        {
            var album = id.HasValue
                ? await _albumRepository.GetByIdAsync(id.Value)
                : CreateNewAlbum();

            InitializeAlbum(album);
        }

        protected async override void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete {Album.Title}?", "Question");
            if (result == MessageDialogResult.Ok)
            {
                _albumRepository.Remove(Album.Model);
                await _albumRepository.SaveAsync();
                RaiseDetailDeletedEvent(Album.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Album != null && !Album.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await _albumRepository.SaveAsync();
            HasChanges = _albumRepository.HasChanges();
            RaiseDetailSavedEvent(Album.Id, Album.Title);
        }

        private void InitializeAlbum(object album)
        {
            Album = new AlbumWrapper((Album)album);
            Album.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _albumRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Album.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if(Album.Id == 0)
            {
                Album.Title = "";
            }
        }

        private Album CreateNewAlbum()
        {
            var album = new Album
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };

            _albumRepository.Add(album);
            return album;
        }
    }
}
