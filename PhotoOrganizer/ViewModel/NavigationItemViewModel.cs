namespace PhotoOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _title;
        public NavigationItemViewModel(int id, string title)
        {
            Id = id;
            _title = title;
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
    }
}
