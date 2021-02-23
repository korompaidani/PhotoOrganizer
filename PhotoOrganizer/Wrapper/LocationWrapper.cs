using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.UI.Wrapper
{
    public class LocationWrapper : NotifyDataErrorInfoBase
    {
        public Location Model { get; }
        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return Model.Coordinates.ToString(); }
            set
            {
                Model.Coordinates = value;
                OnPropertyChanged();
                ValidateProperty("Coordinates", Model.Coordinates);
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

        public LocationWrapper(Location model)
        {
            Model = model;
        }
    }
}
