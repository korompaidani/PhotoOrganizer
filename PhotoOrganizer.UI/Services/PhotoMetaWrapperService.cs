﻿using Autofac;
using Autofac.Features.Indexed;
using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.Resources.Language;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.View.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public class PhotoMetaWrapperService : IPhotoMetaWrapperService
    {
        private IPhotoRepository _photoRepository;
        private IExifReaderWriter _exifToFileWriter;
        private IMessageDialogService _messageDialogService;
        private ApplicationContext _context;

        public PhotoMetaWrapperService(IPhotoRepository photoRepository, 
            IIndex<string, IExifReaderWriter> exifToFileWriter, 
            IMessageDialogService messageDialogService)
        {
            _photoRepository = photoRepository;
            _exifToFileWriter = exifToFileWriter[nameof(MyExifReaderWriter)];
            _messageDialogService = messageDialogService;
            _context = Bootstrapper.Container.Resolve<ApplicationContext>();
        }

        public bool WriteMetaInfoToSingleFile(Photo photoModel, string targetFile)
        {
            var fullPath = Path.GetFullPath(targetFile);
            var properties = MapModelToMetaProperty(photoModel);
            return _exifToFileWriter.WriteMetaToImageFile(fullPath, properties);
        }

        public async Task WriteMetaInfoToAllFileAsync()
        {
            var allPhotos = await _photoRepository.GetModifiedPhotosAsync();

            foreach(var photo in allPhotos)
            {
                var result = await Task<bool>.Run(() => WriteMetaInfoToSingleFile(photo, photo.FullPath));
                if (result)
                {
                    photo.ColorFlag = ColorSign.Finalized;
                }
                else
                {
                    _context.AddErrorMessage(ErrorTypes.MetaWritingError, String.Format(TextResources.DidntSave_errorMessage, photo.Title, photo.Id));
                }
            }

            await _photoRepository.SaveAsync();

        }

        public Photo CreatePhotoModelFromFile(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);
            var result = _exifToFileWriter.ReadMeta(fullPath);

            var date = CreatePhotoComformTakenDate(result);
            var time = new TimeSpan(date.Hour, date.Minute, date.Second);
            var title = CreatePhotoComformTitle(result);
            if (String.IsNullOrEmpty(title))
            {
                title = Path.GetFileNameWithoutExtension(filePath);
            }

            return new Photo
            {
                FullPath = fullPath,
                Title = title,
                Coordinates = CreatePhotoComformCoordinates(result),
                Year = date.Year,
                Month = date.Month,
                Day = date.Day,
                HHMMSS = time,
                Description = CreatePhotoComformDescription(result),
                Creator = CreatePhotoComformCreator(result),
                Comment = CreatePhotoComformPeoples(result)
            };
        }

        string CreatePhotoComformCoordinates(Dictionary<MetaProperty, string> rawData)
        {
            string latitude;
            string longitude;

            rawData.TryGetValue(MetaProperty.Latitude, out latitude);
            rawData.TryGetValue(MetaProperty.Longitude, out longitude);

            if (latitude == null || longitude == null)
            {
                return null;
            }

            var sb = new StringBuilder(latitude);
            sb.Append(",");
            sb.Append(longitude);
            return sb.ToString();
        }

        string CreatePhotoComformTitle(Dictionary<MetaProperty, string> rawData)
        {
            string title = null;

            rawData.TryGetValue(MetaProperty.Title, out title);

            return title;
        }

        string CreatePhotoComformDescription(Dictionary<MetaProperty, string> rawData)
        {
            string description = null;

            rawData.TryGetValue(MetaProperty.Desciprion, out description);

            return description;
        }

        string CreatePhotoComformCreator(Dictionary<MetaProperty, string> rawData)
        {
            string creator = null;

            rawData.TryGetValue(MetaProperty.Author, out creator);

            return creator;
        }

        string CreatePhotoComformPeoples(Dictionary<MetaProperty, string> rawData)
        {
            string peoples = null;

            rawData.TryGetValue(MetaProperty.Keywords, out peoples);

            return peoples;
        }

        DateTime CreatePhotoComformTakenDate(Dictionary<MetaProperty, string> rawData)
        {
            string dateString = null;
            string cleanedDateString = null;

            rawData.TryGetValue(MetaProperty.DateTime, out dateString);
            
            if(dateString != null)
            {
                cleanedDateString = dateString.Remove(dateString.Length - 1, 1);
            }

            DateTime date;

            DateTime.TryParse(cleanedDateString, out date);

            if(date.Year == 1)
            {
                date = new DateTime(1986, 05, 02);
            }

            return date;
        }

        private Dictionary<MetaProperty, string> MapModelToMetaProperty(Photo photoModel)
        {
            var properties = new Dictionary<MetaProperty, string>();

            if (photoModel.Coordinates != null)
            {
                var coordinates = photoModel.Coordinates.Split(',');
                string latitude = string.Empty;
                string longitude = string.Empty;

                if (coordinates != null && coordinates.Length == 2)
                {
                    latitude = coordinates[0];
                    longitude = coordinates[1];
                }

                properties.Add(MetaProperty.Latitude, latitude);
                properties.Add(MetaProperty.Longitude, longitude);
            }

            if (photoModel.Title != null)
            {
                properties.Add(MetaProperty.Title, photoModel.Title);
            }

            if (photoModel.Peoples != null && photoModel.Peoples.Count > 0)
            {
                var sb = new StringBuilder();
                int counter = 0;
                foreach (var people in photoModel.Peoples)
                {
                    counter++;
                    sb.Append(people.DisplayName);
                    if (counter != photoModel.Peoples.Count)
                    {
                        sb.Append(",");
                    }
                }
                properties.Add(MetaProperty.Keywords, sb.ToString());
            }

            if (photoModel.Description != null)
            {
                properties.Add(MetaProperty.Desciprion, photoModel.Description);
            }

            if (photoModel.Creator != null)
            {
                properties.Add(MetaProperty.Author, photoModel.Creator);
            }

            if (photoModel.Year > 0)
            {
                var date = new DateTime(
                    year: photoModel.Year,
                    month: photoModel.Month,
                    day: photoModel.Day,
                    hour: photoModel.HHMMSS.Hours,
                    minute: photoModel.HHMMSS.Minutes,
                    second: photoModel.HHMMSS.Seconds);

                var takenDate = date.ToString("yyyy.MM.dd HH:mm:ss") + '\0';
                properties.Add(MetaProperty.DateTime, takenDate);
            }

            return properties;
        }
    }
}