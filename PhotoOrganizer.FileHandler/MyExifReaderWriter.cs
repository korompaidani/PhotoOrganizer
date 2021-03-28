using PhotoOrganizer.Common;
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

                    Image img = Image.FromStream(memoryStream);

                    foreach (var property in properties)
                    {
                        PropertyItem prop = img.PropertyItems[0];
                        SetProperty(ref prop, property.Key, property.Value);
                        img.SetPropertyItem(prop);
                    }

                    img.Save(fullPath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetProperty(ref PropertyItem propertyItem, MetaProperty propertyId, string propertyValue)
        {
            int id = (int)propertyId;
            int length = propertyValue.Length + 1;
            byte[] propertyValueByteArray = new Byte[length];
            for (int i = 0; i < length - 1; i++)
                propertyValueByteArray[i] = (byte)propertyValue[i];
            propertyValueByteArray[length - 1] = 0x00;
            propertyItem.Id = id;
            propertyItem.Type = 2;
            propertyItem.Value = propertyValueByteArray;
            propertyItem.Len = length;
        }
    }
}
