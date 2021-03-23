using ExifLibrary;
using PhotoOrganizer.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoOrganizer.FileHandler
{
    public class ExifLibraryReaderWriter : IExifReaderWriter
    {
        public Dictionary<MetaProperty, string> ReadMeta(string filepath)
        {
            var result = new Dictionary<MetaProperty, string>();
            using (var memoryStream = new MemoryStream())
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                {
                    fs.CopyTo(memoryStream);
                }

                var image = ImageFile.FromStream(memoryStream);

                foreach (MetaProperty propertyItem in (MetaProperty[])Enum.GetValues(typeof(MetaProperty)))
                {
                    var meta = image.Properties.Get<ExifProperty>((ExifTag)propertyItem).ToString();
                    if (!string.IsNullOrEmpty(meta))
                    {
                        result.Add(propertyItem, meta);
                    }
                }
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

                    var image = ImageFile.FromStream(memoryStream);

                    foreach (var property in properties)
                    {
                        image.Properties.Set((ExifTag)MetaProperty.Latitude, property);
                    }

                    image.Save(fullPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private byte[] ConvertPropertyToByteArray(MetaProperty propertyId, string propertyValue)
        {
            int length = propertyValue.Length + 1;
            byte[] propertyValueByteArray = new Byte[length];
            for (int i = 0; i < length - 1; i++)
                propertyValueByteArray[i] = (byte)propertyValue[i];
            propertyValueByteArray[length - 1] = 0x00;
            return propertyValueByteArray;
        }
    }
}
