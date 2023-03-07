using PhotoOrganizer.Common;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public interface IMetaConverter
    {
        MetaProperty MetaType { get; set; }

        string ConvertMetaToProperty(PropertyItem meta, Image image);

        void ConvertPropertyToMeta(ref Image image, string propertyValue);
    }
}
