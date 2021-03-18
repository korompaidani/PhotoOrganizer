using PhotoOrganizer.Common;
using PhotoOrganizer.Image;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.StateMachine;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoNavigationItemViewModel : ViewModelBase
    {
        private string _displayMemberItem;
        private string _path;
        private string _colorFlag;
        private string _originalColorFlag;
        private Picture _picture;
        private bool _isChecked;
        private IEventAggregator _eventAggregator;
        private string _detailViewModelName;
        private IBulkAttributeSetterService _bulkAttributeSetter;

        public ICommand OpenDetailViewCommand { get; }

        public PhotoNavigationItemViewModel(int id, string displayMemberItem, string path, string colorFlag,
            string detailViewModelName,
            IEventAggregator eventAggregator,
            IBulkAttributeSetterService bulkAttributeSetter)
        {
            Id = id;
            _displayMemberItem = displayMemberItem;
            _path = path;
            _colorFlag = colorFlag;
            _originalColorFlag = _colorFlag;
            _picture = new Picture(_path);
            _eventAggregator = eventAggregator;
            _detailViewModelName = detailViewModelName;
            _bulkAttributeSetter = bulkAttributeSetter;

            IsChecked = _bulkAttributeSetter.IsCheckedById(Id);
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

        public string PhotoPath
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public string ColorFlag
        {
            get { return _colorFlag; }
            set
            {
                _colorFlag = value;
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

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                _bulkAttributeSetter.SetCheckedStateForId(Id, _isChecked);
                SetColorFlag();
                OnPropertyChanged();
            }
        }

        public void SetOriginalColorFlag(string color)
        {
            _originalColorFlag = color;
        }

        private void SetColorFlag()
        {
            if (_isChecked)
            {
                ColorFlag = ColorMap.Map[ColorSign.Checked];
            }
            else
            {
                ColorFlag = _originalColorFlag;
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