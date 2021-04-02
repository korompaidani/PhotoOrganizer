using PhotoOrganizer.Common;
using System.Drawing;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public interface IMetaConverter
    {
        MetaProperty MetaType { get; set; }

        string ConvertMetaToProperty(byte[] data);

        void ConvertPropertyToMeta(ref Image image, string propertyValue);
    }
}
