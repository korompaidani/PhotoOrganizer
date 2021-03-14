using Prism.Events;
using System;
using System.Collections.Generic;

namespace PhotoOrganizer.UI.Event
{
    public class AfterBulkSetPhotoDetailAttributesEvent : PubSubEvent<AfterBulkSetPhotoDetailAttributesEventArgs>
    {
    }

    public class AfterBulkSetPhotoDetailAttributesEventArgs
    {
        /// <summary>
        /// [0] int Id [1] string Title [2] string ColorFlag [3] string PhotoPath
        /// </summary>
        public IList<Tuple<int, string, string, string>> NavigationAttributes { get; set; }
    }
}