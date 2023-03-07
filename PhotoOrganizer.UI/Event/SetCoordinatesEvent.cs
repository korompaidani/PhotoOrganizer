using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class SetCoordinatesEvent : PubSubEvent<SetCoordinatesEventArgs>
    {
    }
    public class SetCoordinatesEventArgs
    {
        public int LocationId { get; set; }
        public int PhotoId { get; set; }
        public string Coordinates { get; set; }
    }
}