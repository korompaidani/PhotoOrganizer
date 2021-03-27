using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class AfterDetailRemovedEvent : PubSubEvent<AfterDetailRemovedEventArgs>
    {
    }

    public class AfterDetailRemovedEventArgs
    {
        public int PhotoId { get; set; }
    }
}