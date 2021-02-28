using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class SaveCoordinatesEvent : PubSubEvent<SaveCoordinatesEventArgs>
    {
    }

    public class SaveCoordinatesEventArgs
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string Coordinates { get; set; }
        public string ViewModelName { get; set; }
    }
}