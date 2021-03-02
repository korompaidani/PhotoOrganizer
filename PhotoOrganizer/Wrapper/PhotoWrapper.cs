﻿using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace PhotoOrganizer.UI.Wrapper
{
    public class PhotoWrapper : NotifyDataErrorInfoBase
    {
        public event LocationChangedEvent LocationChanged;
        public delegate void LocationChangedEvent(object sender, LocationChangedEventArgs args);

        public Photo Model { get; }
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

        public string FullPath
        {
            get { return Model.FullPath; }
            set
            {
                Model.FullPath = value;
                OnPropertyChanged();
                ValidateProperty(nameof(FullPath), Model.FullPath);
            }
        }
        public string Coordinates
        {
            get { return Model.Coordinates; }
            set
            {
                Model.Coordinates = value;                
                OnPropertyChanged();
            }
        }

        public int? LocationId
        {
            get { return Model.LocationId; }
            set
            {
                Model.LocationId = value;
                OnPropertyChanged();
                OnLocationChanged();
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


        public PhotoWrapper(Photo model)
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

            foreach(var result in results)
            {
                AddError(propertyName, result.ErrorMessage);
            }

            // 2. Validate User errors
            switch (propertyName)
            {
                case nameof(Title):
                    if (String.Equals(Title, "xd", StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(propertyName, "Title cannot be xd");
                    }
                    break;
                case nameof(FullPath):
                    // TODO: Validate that Does the file exist
                    break;
            }

        }

        private void OnLocationChanged([CallerMemberName] string propertyName = null)
        {
            if(propertyName == "LocationId")
            {
                LocationChangedEvent handler = LocationChanged;
                handler?.Invoke(this, new LocationChangedEventArgs { PropertyName = propertyName });
            }
        }
    }
}
