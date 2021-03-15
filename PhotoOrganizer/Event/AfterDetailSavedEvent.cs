using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventArgs>
    {
    }

    public class AfterDetailSavedEventArgs
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string Title { get; set; }
        public string PhotoPath { get; set; }
        public string ColorFlag { get; set; }
        public string ViewModelName { get; set; }
        public bool IsShelveChanges { get; set; }
        public bool IsRemovingFromShelve { get; set; }
    }
}
