using PhotoOrganizer.UI.Helpers;
using Prism.Events;
using System.Collections.Generic;

namespace PhotoOrganizer.UI.Event
{
    public class AfterTabClosedEvent : PubSubEvent<AfterTabClosedEventArgs>
    {
    }

    public class AfterTabClosedEventArgs
    {
        public List<PhotoDetailInfo> DetailInfo { get; set; }
    }
}