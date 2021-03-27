using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class SaveCoordinatesAsOverrideEvent : PubSubEvent<SaveCoordinatesAsOverrideEventArgs>
    {
    }

    public class SaveCoordinatesAsOverrideEventArgs
    {
        public int LocationId { get; set; }
        public int PhotoId { get; set; }
        public string LocationName { get; set; }
        public string Coordinates { get; set; }
        public string ViewModelName { get; set; }
    }
}
