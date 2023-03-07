using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class LatitudeConverter : CoordinatesConverterBase
    {
        public LatitudeConverter()
        {
            MetaType = MetaProperty.Latitude;
            DirMetaType = HiddenMetaProperty.LatitudeDir;
            PositiveDirection = 'N';
            NegativeDirection = 'S';
            CoordinateIndex = 0;
        }
    }
}
