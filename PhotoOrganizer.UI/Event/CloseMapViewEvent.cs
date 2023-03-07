using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class CloseMapViewEvent : PubSubEvent<CloseMapViewEventArgs>
    {
    }

    public class CloseMapViewEventArgs
    {
        public string ViewModel { get; set; }
    }
}