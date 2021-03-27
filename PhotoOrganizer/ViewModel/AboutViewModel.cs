using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Diagnostics;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class AboutViewModel : ViewModelBase, ISettingsViewModel
    {
        private IEventAggregator _eventAggregator;
        public ICommand OpenWorkbenchCommand { get; }
        public ICommand GoToLinkCommand { get; }

        public AboutViewModel(
             IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
            GoToLinkCommand = new DelegateCommand<string>(OnGotToLinkExecute);
        }

        private void OnGotToLinkExecute(string link)
        {
            Process.Start(link);
        }

        private void OnOpenWorkbench()
        {
            _eventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());
        }
    }
}
