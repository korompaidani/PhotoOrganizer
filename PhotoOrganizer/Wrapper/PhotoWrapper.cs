using PhotoOrganizer.Model;
using System;

namespace PhotoOrganizer.Wrapper
{
    public class PhotoWrapper : NotifyDataErrorInfoBase
    {
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

        private void ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(Title):
                    if (String.Equals(Title, String.Empty, StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(propertyName, "Title cannot be empty");
                    }
                    break;
            }

        }

        public PhotoWrapper(Photo model)
        {
            Model = model;
        }
    }
}
