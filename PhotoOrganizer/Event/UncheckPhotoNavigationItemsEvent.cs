using Prism.Events;
using System.Collections.Generic;

namespace PhotoOrganizer.UI.Event
{
    public class UncheckPhotoNavigationItemsEvent : PubSubEvent<UncheckPhotoNavigationItemsEventArgs>
    {
    }

    public class UncheckPhotoNavigationItemsEventArgs
    {
        public UncheckPhotoNavigationItemsEventArgs()
        {
            Ids = new HashSet<int>();
        }

        public HashSet<int> Ids { get; set; }
    }
}