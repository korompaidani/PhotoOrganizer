using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class SelectionChangedEvent : PubSubEvent<SelectionChangedEventArgs>
    {
    }

    public class SelectionChangedEventArgs
    {
        public bool IsAnySelectedItem { get; set; }
    }
}