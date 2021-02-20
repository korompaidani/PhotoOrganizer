﻿using PhotoOrganizer.Image;
using PhotoOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMemberItem;
        private Picture _picture;
        private IEventAggregator _eventAggregator;
        private string _detailViewModelName;

        public ICommand OpenDetailViewCommand { get; }

        public NavigationItemViewModel(int id, string displayMemberItem,
            string detailViewModelName,
            IEventAggregator eventAggregator)
        {
            Id = id;
            _displayMemberItem = displayMemberItem;
            _picture = null;
            _eventAggregator = eventAggregator;
            _detailViewModelName = detailViewModelName;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
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

        public Picture Picture
        {
            get { return _picture; }
            set
            {
                _picture = value;
                OnPropertyChanged();
            }
        }

        private void OnOpenDetailViewExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>().
                Publish(
                    new OpenDetailViewEventArgs 
                    {
                        Id = Id,
                        ViewModelName = _detailViewModelName
                    });
        }
    }
}
