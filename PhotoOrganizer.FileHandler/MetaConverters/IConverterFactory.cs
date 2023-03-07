using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public interface IConverterFactory
    {
        IMetaConverter GetMetaConverter(MetaProperty metaProperty);
    }
}