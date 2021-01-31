using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMemberItem;
        private IEventAggregator _eventAggregator;

        public ICommand OpenPhotoDetailViewCommand { get; }

        public NavigationItemViewModel(int id, string displayMemberItem,
            IEventAggregator eventAggregator)
        {
            Id = id;
            _displayMemberItem = displayMemberItem;
            _eventAggregator = eventAggregator;
            OpenPhotoDetailViewCommand = new DelegateCommand(OnOpenPhotoDetailView);
        }               

        public int Id { get; }

        public string DisplayMemberItem
        {
            get { return _displayMemberItem; }
            set
            {
                _displayMemberItem = value;
                OnPropertyChanged();
            }
        }

        private void OnOpenPhotoDetailView()
        {
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Publish(Id);
        }
    }
}
