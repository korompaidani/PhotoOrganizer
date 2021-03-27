using PhotoOrganizer.UI.Data.Repositories;
using Prism.Events;
using System.Collections.Generic;

namespace PhotoOrganizer.UI.Event
{
    public class BulkSetPhotoDetailAttributesEvent : PubSubEvent<BulkSetPhotoDetailAttributesEventArgs>
    {
    }

    public class BulkSetPhotoDetailAttributesEventArgs
    {
        public BulkSetPhotoDetailAttributesEventArgs()
        {
            PropertyNamesAndValues = new Dictionary<string, object>();
        }

        public int CallerId { get; set; }
        public IDictionary<string, object> PropertyNamesAndValues { get; set; }
        public IPhotoRepository PhotoRepository { get; set; }
    }
}
