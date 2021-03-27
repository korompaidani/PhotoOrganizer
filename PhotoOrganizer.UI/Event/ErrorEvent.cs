using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class ErrorEvent : PubSubEvent<ErrorEventArgs>
    {
    }

    public class ErrorEventArgs
    {
        public string ErrorMessage { get; set; }
    }
}