using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class LatitudeConverter : CoordinatesConverterBase
    {
        public LatitudeConverter()
        {
            MetaType = MetaProperty.Latitude;
            DirMetaType = MetaProperty.LatitudeDir;
            PositiveDirection = 'N';
            NegativeDirection = 'S';
            CoordinateIndex = 0;
        }
    }
}
