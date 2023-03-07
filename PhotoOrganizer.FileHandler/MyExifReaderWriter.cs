using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler.MetaConverters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PhotoOrganizer.FileHandler
{
    public class MyExifReaderWriter : IExifReaderWriter
    {
        private IConverterFactory _converterFactory;
        private bool _isLongLat = false;

        public List<string> ErrorMessages { get; }

        public MyExifReaderWriter(IConverterFactory converterFactory)
        {
            _converterFactory = converterFactory;
            ErrorMessages = new List<string>();
        }

        public Dictionary<MetaProperty, string> ReadMeta(string filepath)
        {
            var result = new Dictionary<MetaProperty, string>();

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        fs.CopyTo(memoryStream);
                    }

                    Image image = Image.FromStream(memoryStream);

                    PropertyItem[] imagePropertyItems = image.PropertyItems;

                    foreach (PropertyItem propertyItem in imagePropertyItems)
                    {
                        string property = string.Empty;

                        if (Enum.IsDefined(typeof(MetaProperty), propertyItem.Id)) 
                        {
                            var converter = _converterFactory.GetMetaConverter((MetaProperty)propertyItem.Id);
                            if (converter != null)
                            {
                                try
                                {
                                    property = converter.ConvertMetaToProperty(propertyItem, image);
                                }
                                catch (ArgumentException)
                                {
                                    CheckIfLatitudeOrLongitude((MetaProperty)propertyItem.Id);
                                    ErrorMessages.Add(filepath + $"({nameof(ArgumentException)}) : " + ((MetaProperty)propertyItem.Id).ToString());
                                    continue;
                                }
                            }

                            if (string.IsNullOrEmpty(property))
                            {
                                CheckIfLatitudeOrLongitude((MetaProperty)propertyItem.Id);
                                ErrorMessages.Add(filepath + $"({nameof(BadImageFormatException)}) : " + ((MetaProperty)propertyItem.Id).ToString());
                                continue;
                            }
                            
                            result.Add((MetaProperty)propertyItem.Id, property);
                        }                        
                    }
                }
            }            
            catch(Exception ex)
            {
                throw ex;
            }

            CleanResultIfNecessary(result);
            return result;
        }        

        public bool WriteMetaToImageFile(string fullPath, Dictionary<MetaProperty, string> properties)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                    {
                        fs.CopyTo(memoryStream);
                    }

                    Image image = Image.FromStream(memoryStream);

                    foreach (var property in properties)
                    {
                        var converter = _converterFactory.GetMetaConverter(property.Key);
                        if(converter != null)
                        {
                            converter.ConvertPropertyToMeta(ref image, property.Value);
                        }
                    }

                    image.Save(fullPath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckIfLatitudeOrLongitude(MetaProperty wrongProperty)
        {
            if (wrongProperty == MetaProperty.Latitude || wrongProperty == MetaProperty.Longitude)
            {
                _isLongLat = true;
            }
        }

        private void CleanResultIfNecessary(Dictionary<MetaProperty, string> result)
        {
            if (_isLongLat)
            {
                _isLongLat = false;
                result.Remove(MetaProperty.Latitude);
                result.Remove(MetaProperty.Longitude);
            }
        }
    }
}
