using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class OpenPhotoViewEvent : PubSubEvent<OpenPhotoViewEventArgs>
    {
    }

    public class OpenPhotoViewEventArgs
    {
        public int Id { get; set; }
        public string FullPath { get; set; }
        public string ViewModelName { get; set; }
    }
}
