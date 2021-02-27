using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class AfterSaveCoordinatesEvent : PubSubEvent<AfterSaveCoordinatesEventArgs>
    {
    }
    public class AfterSaveCoordinatesEventArgs
    {
        public int Id { get; set; }
        public string Coordinates { get; set; }
    }
}