using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _title;
        private IEventAggregator _eventAggregator;

        public ICommand OpenPhotoDetailViewCommand { get; }

        public NavigationItemViewModel(int id, string title,
            IEventAggregator eventAggregator)
        {
            Id = id;
            _title = title;
            _eventAggregator = eventAggregator;
            OpenPhotoDetailViewCommand = new DelegateCommand(OnOpenPhotoDetailView);
        }               

        public int Id { get; }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private void OnOpenPhotoDetailView()
        {
            _eventAggregator.GetEvent<OpenPhotoDetailViewEvent>().Publish(Id);
        }
    }
}
