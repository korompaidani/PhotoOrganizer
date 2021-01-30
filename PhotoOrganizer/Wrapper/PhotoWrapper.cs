using PhotoOrganizer.Model;
using PhotoOrganizer.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.Wrapper
{
    public class PhotoWrapper : ViewModelBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errorsByPropertyName
            = new Dictionary<string, List<string>>();

        public Photo Model { get; }
        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return Model.Title; }
            set 
            { 
                Model.Title = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Title));
            }
        }

        public string FullPath
        {
            get { return Model.FullPath; }
            set
            {
                Model.FullPath = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Title));
            }
        }

        public int Year
        {
            get { return Model.Year; }
            set
            {
                Model.Year = value;
                OnPropertyChanged();
            }
        }

        public int Month
        {
            get { return Model.Month; }
            set
            {
                Model.Month = value;
                OnPropertyChanged();
            }
        }

        public int Day
        {
            get { return Model.Day; }
            set
            {
                Model.Day = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan HHMMSS
        {
            get { return Model.HHMMSS; }
            set
            {
                Model.HHMMSS = value;
            }
        }

        public bool HasErrors => _errorsByPropertyName.Any();

        public PhotoWrapper(Photo model)
        {
            Model = model;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName)
                ? _errorsByPropertyName[propertyName]
                : null;
        }

        private void ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(Title):
                    if(String.Equals(Title, String.Empty, StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(propertyName, "Title cannot be empty");
                    }
                    break;
            }

        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }
            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
