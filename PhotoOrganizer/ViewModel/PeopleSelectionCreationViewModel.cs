using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using System.Collections.ObjectModel;
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
        
        public ObservableCollection<PeopleWrapper> Peoples { get; }

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
            Peoples = peoples;
            _detailView = detailView;
            AddPeopleCommand = new DelegateCommand(OnAddPeopleExecute);
            RemovePeopleCommand = new DelegateCommand(OnRemovePeopleExecute, OnRemovePeopleCanExecute);
        }

        private bool OnRemovePeopleCanExecute()
        {
            return SelectedPeople != null;
        }

        private void OnRemovePeopleExecute()
        {
            SelectedPeople.PropertyChanged -= _detailView.PeopleWrapper_PropertyChanged;
            _photoRepository.RemovePeople(SelectedPeople.Model);
            Peoples.Remove(SelectedPeople);
            SelectedPeople = null;
            _detailView.HasChanges = _photoRepository.HasChanges();
            ((DelegateCommand)_detailView.SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPeopleExecute()
        {
            var newPeople = new PeopleWrapper(new People());
            newPeople.PropertyChanged += _detailView.PeopleWrapper_PropertyChanged;
            Peoples.Add(newPeople);
            _detailView.Photo.Model.Peoples.Add(newPeople.Model);
            newPeople.DisplayName = "";
        }
    }
}
