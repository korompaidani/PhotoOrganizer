using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class AfterPeopleDeletedEvent : PubSubEvent<AfterPeopleDeletedEventArgs>
    {
    }

    public class AfterPeopleDeletedEventArgs
    {
        public int Id { get; set; }
    }
}
