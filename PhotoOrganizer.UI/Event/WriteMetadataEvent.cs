using PhotoOrganizer.Model;
using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class WriteMetadataEvent : PubSubEvent<WriteMetadataEventArgs>
    {
    }

    public class WriteMetadataEventArgs
    {
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}