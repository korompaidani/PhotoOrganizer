using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class YearDetailViewModel : DetailViewModelBase
    {
        private IYearRepository _yearRepository;
        private YearWrapper _selectedYear;

        public ObservableCollection<YearWrapper> Years { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public YearWrapper SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public YearDetailViewModel(IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService,
            IYearRepository yearRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _yearRepository = yearRepository;
            Title = "Years";
            Years = new ObservableCollection<YearWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        public async override Task LoadAsync(int id)
        {
            Id = id;
            foreach(var wrapper in Years)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            Years.Clear();

            var years = await _yearRepository.GetAllAsync();

            foreach(var model in years)
            {
                var wrapper = new YearWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                Years.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _yearRepository.HasChanges();
            }
            if(e.PropertyName == nameof(YearWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && Years.All(y => !y.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            try
            {
                await _yearRepository.SaveAsync();
                HasChanges = _yearRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch(Exception ex)
            {
                while(ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                await MessageDialogService.ShowInfoDialogAsync("Error while saving the entities, " +
                    "the data will be reloaded. Details: " + ex.Message);
                await LoadAsync(Id);
            }
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedYear != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced = await _yearRepository.IsReferencedByPhotoAsync(SelectedYear.Id);
            if (isReferenced)
            {
                await MessageDialogService.ShowInfoDialogAsync($"The year {SelectedYear.Title} can't be removed, as it is referenced by at least one photo");
                return;
            }

            SelectedYear.PropertyChanged -= Wrapper_PropertyChanged;
            _yearRepository.Remove(SelectedYear.Model);
            Years.Remove(SelectedYear);
            SelectedYear = null;
            HasChanges = _yearRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new YearWrapper(new Year());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _yearRepository.Add(wrapper.Model);
            Years.Add(wrapper);

            // Trigger the validation
            wrapper.Title = "1900";
        }
    }
}
