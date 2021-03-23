using PhotoOrganizer.Common;
using System.Collections.Generic;

namespace PhotoOrganizer.FileHandler
{
    public interface IExifReaderWriter
    {
        Dictionary<MetaProperty, string> ReadMeta(string filepath);
        bool WriteMetaToImageFile(string fullPath, Dictionary<MetaProperty, string> properties);
    }
}