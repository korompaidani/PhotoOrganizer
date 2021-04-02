using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class TakenDateConverter : ConverterBase
    {
        public TakenDateConverter()
        {
            MetaType = MetaProperty.DateTime;
        }
    }
}
