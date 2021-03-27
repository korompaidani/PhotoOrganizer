using PhotoOrganizer.UI.ViewModel;
using Prism.Events;

namespace PhotoOrganizer.UI.Event
{
    public class OpenDetailViewEvent : PubSubEvent<OpenDetailViewEventArgs>
    {
    }

    public class OpenDetailViewEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
        public IDetailViewModel DetailView { get; set; }
    }
}
