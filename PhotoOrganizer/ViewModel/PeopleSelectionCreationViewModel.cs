using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
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
        private IPhotoRepository _photoRepository;
        private IPeopleRepository _peopleRepository;
        public ICommand AddPeopleCommand { get; }
        public ICommand RemovePeopleCommand { get; }
        public ICommand AddSelectedPeopleToPhotoCommand { get; }

        public ObservableCollection<PeopleWrapper> Peoples { get; }
        public ObservableCollection<PeopleWrapper> PeoplesOnPhoto { get; }

        public PeopleWrapper SelectedPeople
        {
            get { return _selectedPeople; }
            set
            {
                _selectedPeople = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePeopleCommand).RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand { get; private set; }

        public PeopleSelectionCreationViewModel(
            IPhotoRepository photoRepository, 
            IPeopleRepository peopleRepository,
            ObservableCollection<PeopleWrapper> peoples, 
            PhotoDetailViewModel detailView)
        {
            _photoRepository = photoRepository;
            _peopleRepository = peopleRepository;
            PeoplesOnPhoto = peoples;
            Peoples = new ObservableCollection<PeopleWrapper>();
            _detailView = detailView;
            AddPeopleCommand = new DelegateCommand(OnAddPeopleExecute);
            RemovePeopleCommand = new DelegateCommand(OnRemovePeopleExecute, OnRemovePeopleCanExecute);

            AddSelectedPeopleToPhotoCommand = new DelegateCommand(OnAddSelectedPeopleToPhotoExecute, OnAddSelectedPeopleToPhotoCanExecute);
        }

        private bool OnAddSelectedPeopleToPhotoCanExecute()
        {
            return true;
            //return SelectedPeople != null;
        }

        private void OnAddSelectedPeopleToPhotoExecute()
        {
            if (!PeoplesOnPhoto.Any(p => p.Id == SelectedPeople.Model.Id))
            {
                PeoplesOnPhoto.Add(SelectedPeople);
                _detailView.Photo.Model.Peoples.Add(SelectedPeople.Model);
                _detailView.HasChanges = _photoRepository.HasChanges();
                ((DelegateCommand)_detailView.SaveCommand).RaiseCanExecuteChanged();
                SelectedPeople = null;
            }
        }

        private bool OnRemovePeopleCanExecute()
        {
            return SelectedPeople != null;
        }

        private void OnRemovePeopleExecute()
        {
            SelectedPeople.PropertyChanged -= _detailView.PeopleWrapper_PropertyChanged;
            _peopleRepository.Remove(SelectedPeople.Model);

            if(PeoplesOnPhoto.Any(p => p.Id == SelectedPeople.Model.Id))
            {
                _photoRepository.RemovePeople(SelectedPeople.Model);
                _detailView.HasChanges = _photoRepository.HasChanges();
                ((DelegateCommand)_detailView.SaveCommand).RaiseCanExecuteChanged();
            }

            Peoples.Remove(SelectedPeople);
            SelectedPeople = null;
        }

        private void OnAddPeopleExecute()
        {
            var newPeople = new PeopleWrapper(new People());
            newPeople.PropertyChanged += _detailView.PeopleWrapper_PropertyChanged;
            Peoples.Add(newPeople);
            PeoplesOnPhoto.Add(newPeople);
            _detailView.Photo.Model.Peoples.Add(newPeople.Model);
            newPeople.DisplayName = "";
        }

        public async Task LoadAsync()
        {
            var allPeople = await _peopleRepository.GetAllAsync();
            foreach(var people in allPeople)
            {
                if(!PeoplesOnPhoto.Any(p => p.Id == people.Id))
                {
                    Peoples.Add(new PeopleWrapper(people));
                }
            }
        }
    }
}
