using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PeopleItemViewModel : ViewModelBase
    {
        private string _displayMemberItem;
        private IEventAggregator _eventAggregator;        
        public ICommand RemovePeopleFromPhotoCommand { get; }
        private PeopleWrapper _people;

        public PeopleItemViewModel(PeopleWrapper people,
            IEventAggregator eventAggregator)
        {
            People = people;
            _eventAggregator = eventAggregator;
            RemovePeopleFromPhotoCommand = new DelegateCommand(OnRemovePeopleFromPhoto);
        }

        public int Id 
        { 
            get
            {
                return People.Id;
            }
        }

        public PeopleWrapper People
        {
            get { return _people; }
            set
            {
                _people = value;
                OnPropertyChanged();
            }
        }

        public string DisplayMemberItem
        {
            get { return _displayMemberItem; }
            set
            {
                _displayMemberItem = value;
                OnPropertyChanged();
            }
        }

        private void OnRemovePeopleFromPhoto()
        {
            _eventAggregator.GetEvent<AfterPeopleDeletedEvent>().
                Publish(new AfterPeopleDeletedEventArgs { Id = this.Id });    
        }
    }
}
