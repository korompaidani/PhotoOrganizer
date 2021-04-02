using PhotoOrganizer.Common;
using System;
using System.Drawing;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class ConverterBase : IMetaConverter
    {
        public MetaProperty MetaType { get; set; }

        public virtual string ConvertMetaToProperty(byte[] data)
        {
            return string.Empty;
        }

        public virtual void ConvertPropertyToMeta(ref Image image, string propertyValue)
        {
            var propertyItem = image.PropertyItems[0];
            int id = (int)MetaType;
            int length = propertyValue.Length + 1;
            byte[] propertyValueByteArray = new Byte[length];

            for (int i = 0; i < length - 1; i++)
            {
                propertyValueByteArray[i] = (byte)propertyValue[i];
            }

            propertyValueByteArray[length - 1] = 0x00;
            propertyItem.Id = id;
            propertyItem.Type = 2;
            propertyItem.Value = propertyValueByteArray;
            propertyItem.Len = length;
            image.SetPropertyItem(propertyItem);
        }
    }
}
