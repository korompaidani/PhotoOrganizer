using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class GetBrowserDataEvent : PubSubEvent<GetBrowserDataEventArgs>
    {
    }
    public class GetBrowserDataEventArgs
    {
        public string Url { get; set; }
    }
}