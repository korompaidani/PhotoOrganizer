using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using PhotoOrganizer.UI.Data.Repositories;
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
        private ExifIO _exifToFileWriter;

        public PhotoMetaWrapperService(IPhotoRepository photoRepository, ExifIO exifToFileWriter)
        {
            _photoRepository = photoRepository;
            _exifToFileWriter = exifToFileWriter;
        }

        public bool WriteMetaInfoToSingleFile(Photo photoModel)
        {
            var fullPath = Path.GetFullPath(photoModel.FullPath);
            var properties = MapModelToMetaProperty(photoModel);
            return _exifToFileWriter.WriteMetaToOriginalImageFile(fullPath, properties);
        }

        public async Task WriteMetaInfoToAllFileAsync()
        {
            var allPhotos = await _photoRepository.GetAllAsync();
            throw new System.NotImplementedException();
        }

        public Photo CreatePhotoFromMeta(string filePath)
        {
            throw new NotImplementedException();
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
                properties.Add(MetaProperty.Comments, sb.ToString());
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

                var takenDate = date.ToString("yyyy:MM:dd HH:mm:ss") + '\0';
                properties.Add(MetaProperty.DateTime, takenDate);
            }

            return properties;
        }
    }
}
