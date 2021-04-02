using PhotoOrganizer.Common;
using PhotoOrganizer.FileHandler.MetaConverters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace PhotoOrganizer.FileHandler
{
    public class MyExifReaderWriter : IExifReaderWriter
    {
        private IConverterFactory _converterFactory;

        public MyExifReaderWriter(IConverterFactory converterFactory)
        {
            _converterFactory = converterFactory;
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
                        if (Enum.IsDefined(typeof(MetaProperty), propertyItem.Id))
                        {
                            result.Add((MetaProperty)propertyItem.Id, Encoding.UTF8.GetString(propertyItem.Value));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

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
    }
}
