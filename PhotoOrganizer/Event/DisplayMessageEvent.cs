using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class DisplayMessageEvent : PubSubEvent<DisplayMessageEventArgs>
    {
    }

    public class DisplayMessageEventArgs
    {
        public string MessageTitle { get; set; }
        public string MessageContent { get; set; }
    }
}