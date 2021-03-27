using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class StartScreenViewModel
    {
        private IEventAggregator _eventAggregator;
        public ICommand OpenWorkbenchCommand { get; }

        public StartScreenViewModel(
             IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);            
        }

        private void OnOpenWorkbench()
        {
            _eventAggregator.GetEvent<CloseMapViewEvent>().
                Publish(new CloseMapViewEventArgs 
                {
                    ViewModel = this.GetType().Name
                });
        }
    }
}
