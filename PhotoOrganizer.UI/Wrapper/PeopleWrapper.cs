using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.UI.Wrapper
{
    public class PeopleWrapper : NotifyDataErrorInfoBase
    {
        public People Model { get; }
        public int Id { get { return Model.Id; } }

        public string DisplayName
        {
            get { return Model.DisplayName; }
            set
            {
                Model.DisplayName = value;
                OnPropertyChanged();
                ValidateProperty(nameof(DisplayName), Model.DisplayName);
            }
        }

        public string FirstName
        {
            get { return Model.FirstName; }
            set
            {
                Model.FirstName = value;
                OnPropertyChanged();
                ValidateProperty(nameof(FirstName), Model.FirstName);
            }
        }

        public PeopleWrapper(People model)
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
        }
    }
}
