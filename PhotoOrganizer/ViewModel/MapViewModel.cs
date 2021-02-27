using PhotoOrganizer.UI.Event;
using PhotoOrganizer.MapTools;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class MapViewModel : DetailViewModelBase, IMapViewModel
    {
        private LocationWrapper _location;
        private string _webUrl = "https://www.google.com/maps";
        public ICommand OnWorkbenchCommand { get; }
        public ICommand OnSetPhotoOnlyCommand { get; }
        public ICommand OnSaveLocationCommand { get; }

        public string WebUrl
        {
            get
            {
                return _webUrl;
            }
            set
            {
                _webUrl = value;
                OnPropertyChanged();
            }
        }

        public LocationWrapper Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }

        public MapViewModel(
             IEventAggregator eventAggregator,
             IMessageDialogService messageDialogService)
            : base(eventAggregator, messageDialogService)
        {
            OnWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
        }

        private void OnOpenWorkbench()
        {
            // TODO: It must be removed to other commands
            //EventAggregator.GetEvent<CloseMapViewEvent>().
            //    Publish(new CloseMapViewEventArgs 
            //    {
            //        LocationName = "newLoc",//Location.LocationName,
            //        Coordinates = "xy"//Location.Coordinates
            //    });

            //EventAggregator.GetEvent<AfterSaveCoordinatesEvent>().
            //    Publish(new AfterSaveCoordinatesEventArgs
            //    {
            //        Id = -1
            //    });

            EventAggregator.GetEvent<OpenWorkbenchViewEvent>().
                Publish(new OpenWorkbenchViewEventArgs());

        }

        public override Task LoadAsync(int id)
        {
            throw new NotImplementedException();
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
