﻿using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventArgs>
    {
    }

    public class AfterDetailSavedEventArgs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ViewModelName { get; set; }
    }
}