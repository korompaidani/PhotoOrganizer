using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.UI.Wrapper
{
    public class YearWrapper : NotifyDataErrorInfoBase
    {
        public Year Model { get; }
        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return Model.PhotoTakenYear.ToString(); }
            set
            {
                Model.PhotoTakenYear = Int32.Parse(value);
                OnPropertyChanged();
                ValidateProperty("PhotoTakenYear", Model.PhotoTakenYear);
            }
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

        public YearWrapper(Year model)
        {
            Model = model;
        }
    }
}
