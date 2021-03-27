using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class HelpViewModel : ViewModelBase, ISettingsViewModel
    {
        private IEventAggregator _eventAggregator;
        public ICommand OpenWorkbenchCommand { get; }

        public HelpViewModel(
             IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
        }

        private void OnOpenWorkbench()
        {
            _eventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs());
        }
    }
}
