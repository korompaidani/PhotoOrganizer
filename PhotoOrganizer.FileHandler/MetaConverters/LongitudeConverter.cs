using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class LongitudeConverter : CoordinatesConverterBase
    {
        public LongitudeConverter()
        {
            MetaType = MetaProperty.Longitude;
            DirMetaType = HiddenMetaProperty.LongitudeDir;
            PositiveDirection = 'E';
            NegativeDirection = 'W';
            CoordinateIndex = 1;
        }
    }
}
