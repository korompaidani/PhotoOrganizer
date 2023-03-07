using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class OpenMapViewEvent : PubSubEvent<OpenMapViewEventArgs>
    {
    }

    public class OpenMapViewEventArgs
    {
        public int PhotoId { get; set; }
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string Coordinates { get; set; }
        public string ViewModelName { get; set; }
    }
}