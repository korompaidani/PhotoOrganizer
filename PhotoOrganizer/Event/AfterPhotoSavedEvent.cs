﻿using Prism.Events;

namespace PhotoOrganizer.Event
{
    public class AfterPhotoSavedEvent : PubSubEvent<AfterPhotoSavedEventArgs>
    {
    }

    public class AfterPhotoSavedEventArgs
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}