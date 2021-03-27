using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.UI.Wrapper
{
    public class AlbumWrapper : NotifyDataErrorInfoBase
    {
        public Album Model { get; }
        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return Model.Title; }
            set
            {
                Model.Title = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Title), Model.Title);
            }
        }

        public DateTime DateFrom
        {
            get { return Model.DateFrom; }
            set
            {
                Model.DateFrom = value;
                OnPropertyChanged();
                if(DateTo < DateFrom)
                {
                    DateTo = DateFrom;
                }
                ValidateProperty(nameof(DateFrom), Model.DateFrom);
            }
        }

        public DateTime DateTo
        {
            get { return Model.DateTo; }
            set
            {
                Model.DateTo = value;
                OnPropertyChanged();
                if (DateTo < DateFrom)
                {
                    DateTo = DateFrom;
                }
                ValidateProperty(nameof(DateTo), Model.DateTo);
            }
        }

        public AlbumWrapper(Album model)
        {
            Model = model;
        }

        private void ValidateProperty(string propertyName, object currentValue)
        {
            ClearErrors(propertyName);

            // 1.Validate Data Annotations
            var results = new List<ValidationResult>();
            var context = new ValidationContext(Model) { MemberName = propertyName };
            Validator.TryValidateProperty(currentValue, context, results);

            foreach (var result in results)
            {
                AddError(propertyName, result.ErrorMessage);
            }

            // 2. Validate User errors                      
        }
    }
}
