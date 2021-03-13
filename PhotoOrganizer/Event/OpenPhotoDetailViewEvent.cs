using PhotoOrganizer.UI.ViewModel;
using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class OpenPhotoDetailViewEvent : PubSubEvent<OpenPhotoDetailViewEventArgs>
    {
    }

    public class OpenPhotoDetailViewEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
        public IDetailViewModel DetailView { get; set; }
    }
}