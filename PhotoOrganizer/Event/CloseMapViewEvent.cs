using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class CloseMapViewEvent : PubSubEvent<CloseMapViewEventArgs>
    {
    }

    public class CloseMapViewEventArgs
    {
        public int PhotoId { get; set; }
        public string LocationName { get; set; }
        public string Coordinates { get; set; }
        public string ViewModelName { get; set; }
    }
}