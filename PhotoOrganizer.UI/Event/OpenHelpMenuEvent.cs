using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class OpenHelpMenuEvent : PubSubEvent<OpenHelpMenuEventArgs>
    {
    }

    public class OpenHelpMenuEventArgs
    {
        public string ViewModelName { get; set; }
    }
}