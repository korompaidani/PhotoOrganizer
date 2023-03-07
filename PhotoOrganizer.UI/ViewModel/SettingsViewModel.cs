using PhotoOrganizer.Common;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private IEventAggregator _eventAggregator;
        private ISettingsHandler _settingsHandler;
        private SettingsWrapper _settings;
        private int _selectedPageSize;
        private string _selectedLanguage;
        private string _actualLanguageSet;
        private int _actualPageSet;
        private bool _hasChanges;

        public ICommand OpenWorkbenchCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand OkCommand { get; }

        public ObservableCollection<int> PageSizes { get; private set; }
        public ObservableCollection<string> Languages { get; private set; }

        public string ActualLanguageSet
        {
            get { return _actualLanguageSet; }
            set
            {
                _actualLanguageSet = value;               
                OnPropertyChanged();
            }
        }

        public int ActualPageSet
        {
            get { return _actualPageSet; }
            set
            {
                _actualPageSet = value;
                OnPropertyChanged();
            }
        }

        public int SelectedPageSize
        {
            get { return _selectedPageSize; }
            set
            {
                _selectedPageSize = value;
                if(Settings != null)
                {
                    Settings.PageSize = _selectedPageSize;
                }

                OnPropertyChanged();
                HasChanges = true;
            }
        }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                if (Settings != null)
                {
                    Settings.Language = _selectedLanguage;
                }

                OnPropertyChanged();
                HasChanges = true;
            }
        }

        public SettingsWrapper Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
            }
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)ApplyCommand).RaiseCanExecuteChanged();
                    ((DelegateCommand)OkCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public SettingsViewModel(
             IEventAggregator eventAggregator,
             ISettingsHandler settingsHandler)
        {
            _eventAggregator = eventAggregator;
            _settingsHandler = settingsHandler;

            OpenWorkbenchCommand = new DelegateCommand(OnOpenWorkbench);
            ApplyCommand = new DelegateCommand(OnApplyExecute, OnApplyCanExecute);
            OkCommand = new DelegateCommand(OnOkExecute, OnOkCanExecute);
            LoadPageSizesCombo();
            LoadLanguagesCombo();
        }

        private bool OnOkCanExecute()
        {
            return HasChanges;
        }

        private async void OnOkExecute()
        {
            await ApplyAndSave();

            _eventAggregator.GetEvent<CloseSettingsEvent>().
                Publish(new CloseSettingsEventArgs());
        }

        private bool OnApplyCanExecute()
        {
            return HasChanges;
        }

        private async void OnApplyExecute()
        {
            await ApplyAndSave();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            if(Settings == null)
            {
                var settings = await _settingsHandler.LoadSettingsAsync();
                if (settings == null)
                {
                    settings = new Settings { PageSize = PageSizes[0] };
                }

                Settings = new SettingsWrapper(settings);
            }

            SelectedPageSize = Settings.PageSize;

            ActualPageSet = Settings.PageSize;
            ActualLanguageSet = Settings.Language;
            HasChanges = false;
        }

        private void OnOpenWorkbench()
        {
            _eventAggregator.GetEvent<CloseSettingsEvent>().
                Publish(new CloseSettingsEventArgs());
        }

        private async Task ApplyAndSave()
        {
            await _settingsHandler.ApplySettingsAsync(Settings.Model);
            await _settingsHandler.SaveSettingsAsync(Settings.Model);
        }

        private void LoadPageSizesCombo()
        {
            var pageSizeCombobox = new List<int>();
            pageSizeCombobox.AddRange(CommonContants.PageSizes);

            PageSizes = new ObservableCollection<int>();

            foreach (var comboItem in pageSizeCombobox)
            {
                PageSizes.Add(comboItem);
            }

            SelectedPageSize = PageSizes[0];            
        }

        private void LoadLanguagesCombo()
        {
            var languagesCombo = new List<string>();
            languagesCombo.AddRange(CommonContants.Languages);

            Languages = new ObservableCollection<string>();

            foreach (var comboItem in languagesCombo)
            {
                Languages.Add(comboItem);
            }

            SelectedLanguage = Languages[0];            
        }
    }
}
