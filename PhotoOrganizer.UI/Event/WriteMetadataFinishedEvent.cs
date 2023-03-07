using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class WriteMetadataFinishedEvent : PubSubEvent<WriteMetadataFinishedEventArgs>
    {
    }

    public class WriteMetadataFinishedEventArgs
    {
        // messages service will it listen also
        public int PhotoId { get; set; }
        public bool IsSuccesfullyDone { get; set; }
    }
}
