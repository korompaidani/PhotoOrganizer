using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.IO;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoViewModel : ViewModelBase, IPhotoViewModel
    {
        private IEventAggregator _eventAggregator;
        public ICommand OpenWorkbenchCommand { get; }
        public string FullPath { get; }

        public PhotoViewModel(
             IEventAggregator eventAggregator,
             string fullPath)
        {
            _eventAggregator = eventAggregator;
            FullPath = Path.GetFullPath(fullPath);
            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
        }

        private void OnOpenWorkbench()
        {
            _eventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());
        }
    }
}
