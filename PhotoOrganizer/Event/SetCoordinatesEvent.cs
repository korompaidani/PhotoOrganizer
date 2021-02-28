using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class SetCoordinatesEvent : PubSubEvent<SetCoordinatesEventArgs>
    {
    }
    public class SetCoordinatesEventArgs
    {
        public int Id { get; set; }
        public string Coordinates { get; set; }
    }
}