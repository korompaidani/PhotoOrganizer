using PhotoOrganizer.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class ConverterBase : IMetaConverter
    {
        public MetaProperty MetaType { get; set; }

        public virtual string ConvertMetaToProperty(PropertyItem meta, Image image)
        {
            if (meta.Id != (int)MetaType) { return null; }

            return Encoding.GetEncoding("iso-8859-2").GetString(meta.Value);
        }

        public virtual void ConvertPropertyToMeta(ref Image image, string propertyValue)
        {
            var propertyItem = image.PropertyItems[0];
            int id = (int)MetaType;
            byte[] propertyValueByteArray = Encoding.GetEncoding("iso-8859-2").GetBytes(propertyValue);
            byte[] biggerPropertyValueByteArray = new byte[propertyValueByteArray.Length + 1];
            propertyValueByteArray.CopyTo(biggerPropertyValueByteArray, 0);
            int length = biggerPropertyValueByteArray.Length;

            biggerPropertyValueByteArray[length - 1] = 0x00;
            propertyItem.Id = id;
            propertyItem.Type = 2;
            propertyItem.Value = biggerPropertyValueByteArray;
            propertyItem.Len = length;
            image.SetPropertyItem(propertyItem);
        }
    }
}
