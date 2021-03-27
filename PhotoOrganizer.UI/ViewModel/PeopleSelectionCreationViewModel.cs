using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PeopleSelectionCreationViewModel : ViewModelBase, IPeopleSelectionCreationViewModel
    {
        private PhotoDetailViewModel _detailView;
        private PeopleWrapper _selectedPeople;
        private string _nameInputTextBox;
        private IPhotoRepository _photoRepository;
        private IEventAggregator _eventAggregator;
        public ICommand AddPeopleCommand { get; }
        public ICommand RemovePeopleCommand { get; }
        public ICommand AddSelectedPeopleToPhotoCommand { get; }

        public ObservableCollection<PeopleWrapper> Peoples { get; }
        public ObservableCollection<PeopleItemViewModel> PeoplesOnPhoto { get; }

        public PeopleWrapper SelectedPeople
        {
            get { return _selectedPeople; }
            set
            {
                _selectedPeople = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePeopleCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)AddSelectedPeopleToPhotoCommand).RaiseCanExecuteChanged();
            }
        }

        public string NameInputTextBox
        {
            get { return _nameInputTextBox; }
            set
            {
                _nameInputTextBox = value;
                OnPropertyChanged();
                ((DelegateCommand)AddPeopleCommand).RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand { get; private set; }

        public PeopleSelectionCreationViewModel(
            IPhotoRepository photoRepository, 
            ObservableCollection<PeopleItemViewModel> peoples, 
            PhotoDetailViewModel detailView,
            IEventAggregator eventAggregator)
        {
            _photoRepository = photoRepository;
            PeoplesOnPhoto = peoples;
            _eventAggregator = eventAggregator;
            Peoples = new ObservableCollection<PeopleWrapper>();
            _detailView = detailView;
            AddPeopleCommand = new DelegateCommand(OnAddPeopleExecute, OnAddPeopleCanExecute);
            RemovePeopleCommand = new DelegateCommand(OnRemovePeopleExecute, OnRemovePeopleCanExecute);

            AddSelectedPeopleToPhotoCommand = new DelegateCommand(OnAddSelectedPeopleToPhotoExecute, OnAddSelectedPeopleToPhotoCanExecute);
        }

        private bool OnAddSelectedPeopleToPhotoCanExecute()
        {
            return SelectedPeople != null;
        }

        private void OnAddSelectedPeopleToPhotoExecute()
        {
            if (!PeoplesOnPhoto.Any(p => p.People.Id == SelectedPeople.Model.Id))
            {
                var peopleItem = new PeopleItemViewModel(new PeopleWrapper(SelectedPeople.Model), _eventAggregator);
                PeoplesOnPhoto.Add(peopleItem);
                _detailView.Photo.AddPeople(SelectedPeople.Model);
                Peoples.Remove(SelectedPeople);
                SelectedPeople = null;
            }
            else
            {
                Peoples.Remove(SelectedPeople);
            }
        }

        private bool OnRemovePeopleCanExecute()
        {
            return SelectedPeople != null;
        }

        private void OnRemovePeopleExecute()
        {
            SelectedPeople.PropertyChanged -= _detailView.PeopleWrapper_PropertyChanged;
            _photoRepository.RemovePeople(SelectedPeople.Model);

            if(PeoplesOnPhoto.Any(p => p.People.Id == SelectedPeople.Model.Id))
            {
                _photoRepository.RemovePeople(SelectedPeople.Model);
                _detailView.HasChanges = _photoRepository.HasChanges();
                ((DelegateCommand)_detailView.SaveCommand).RaiseCanExecuteChanged();
            }

            Peoples.Remove(SelectedPeople);
            SelectedPeople = null;
        }

        private bool OnAddPeopleCanExecute()
        {
            if(!string.IsNullOrEmpty(NameInputTextBox))
            {
                return true;
            }

            return false;
        }

        private void OnAddPeopleExecute()
        {
            var existingPeople = Peoples.FirstOrDefault(p => p.DisplayName == NameInputTextBox);
            var isPeopleOnPhoto = PeoplesOnPhoto.Any(p => p.People.DisplayName == NameInputTextBox);
            if (existingPeople != null || isPeopleOnPhoto)
            {
                SelectedPeople = null;                
                Peoples.Remove(existingPeople);

                if (!isPeopleOnPhoto)
                {
                    var peopleItem = new PeopleItemViewModel(existingPeople, _eventAggregator);
                    PeoplesOnPhoto.Add(peopleItem);
                }
                
                if(existingPeople != null)
                {                
                    _detailView.Photo.AddPeople(existingPeople.Model);
                }
            }
            else
            {
                var newPeople = new PeopleWrapper(new People { DisplayName = NameInputTextBox });
                newPeople.PropertyChanged += _detailView.PeopleWrapper_PropertyChanged;
                var peopleItem = new PeopleItemViewModel(newPeople, _eventAggregator);
                PeoplesOnPhoto.Add(peopleItem);
                _detailView.Photo.AddPeople(newPeople.Model);
            }

            NameInputTextBox = string.Empty;
        }

        public async Task LoadAsync()
        {
            var allPeople = await _photoRepository.GetAllPeopleAsync();
            foreach(var people in allPeople)
            {
                if(!PeoplesOnPhoto.Any(p => p.People.Id == people.Id))
                {
                    Peoples.Add(new PeopleWrapper(people));
                }
            }
        }
    }
}
