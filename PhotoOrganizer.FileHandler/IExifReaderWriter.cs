using PhotoOrganizer.Common;
using System.Collections.Generic;

namespace PhotoOrganizer.FileHandler
{
    public interface IExifReaderWriter
    {
        List<string> ErrorMessages { get; }
        Dictionary<MetaProperty, string> ReadMeta(string filepath);
        bool WriteMetaToImageFile(string fullPath, Dictionary<MetaProperty, string> properties);
    }
}