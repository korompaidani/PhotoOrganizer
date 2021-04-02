using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class TitleConverter : ConverterBase
    {
        public TitleConverter()
        {
            MetaType = MetaProperty.Title;
        }
    }
}
