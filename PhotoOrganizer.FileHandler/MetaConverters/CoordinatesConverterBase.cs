using PhotoOrganizer.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class CoordinatesConverterBase : ConverterBase
    {
        protected HiddenMetaProperty DirMetaType;
        protected char PositiveDirection;
        protected char NegativeDirection;
        protected byte CoordinateIndex;

        public override string ConvertMetaToProperty(PropertyItem meta, Image image)
        {
            if(meta.Id != (int)MetaType) { return null; }

            uint degreesNumerator = BitConverter.ToUInt32(meta.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(meta.Value, 4);
            double degrees = degreesNumerator / (double)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(meta.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(meta.Value, 12);
            double minutes = minutesNumerator / (double)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(meta.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(meta.Value, 20);
            double seconds = secondsNumerator / (double)secondsDenominator;

            double coorditate = degrees + (minutes / 60d) + (seconds / 3600d);

            var dir = image.PropertyItems.FirstOrDefault(p => p.Id == (int)DirMetaType);
            
            if(dir == null)
            {
                return null;
            }

            char[] coordinateDir = System.Text.Encoding.ASCII.GetChars(new byte[1] { dir.Value[0] });

            if(coordinateDir == null || coordinateDir.Length != 1)
            {
                return null; 
            }

            if (coordinateDir[0] == NegativeDirection)
            {
                coorditate *= -1;
            }

            return coorditate.ToString().Replace(",", ".");
        }

        public override void ConvertPropertyToMeta(ref Image image, string propertyValue)
        {
            try
            {
                var cleanedString = propertyValue.Replace("\0", "").Replace(".", ",");
                ConvertValueToDoubleAndSetMeta(ref image, cleanedString);
            }
            catch (FormatException)
            {
                var cleanedString = propertyValue.Replace("\0", "");
                ConvertValueToDoubleAndSetMeta(ref image, cleanedString);
            }
        }

        private void ConvertValueToDoubleAndSetMeta(ref Image image, string cleanedString)
        {
            var propertyValueInDouble = Convert.ToDouble(cleanedString);
            SetCoordinateValue(ref image, propertyValueInDouble);
            SetDirValue(ref image, propertyValueInDouble);
        }

        private void SetCoordinateValue(ref Image image, double propertyValue)
        {
            var propertyItem = image.PropertyItems[0];
            propertyItem.Id = (int)MetaType;
            propertyItem.Type = 5;
            propertyItem.Value = ConvertRationalValue(propertyValue);
            propertyItem.Len = propertyItem.Value.Length;
            image.SetPropertyItem(propertyItem);
        }

        private void SetDirValue(ref Image image, double propertyValue)
        {
            char direction;
            if(propertyValue > 0)
            {
                direction = PositiveDirection;
            }
            else
            {
                direction = NegativeDirection;
            }

            var propertyItem = image.PropertyItems[0];

            propertyItem.Id = (int)DirMetaType;
            propertyItem.Type = 2;
            propertyItem.Value = BitConverter.GetBytes(direction);
            propertyItem.Len = propertyItem.Value.Length;
            image.SetPropertyItem(propertyItem);
        }

        private byte[] ConvertRationalValue(double latitude)
        {
            double temp;
            temp = Math.Abs(latitude);

            uint degreesNumerator = (uint)Math.Truncate(temp);
            temp = (temp - degreesNumerator) * 60;

            uint minutesNumerator = (uint)Math.Truncate(temp);
            temp = (temp - minutesNumerator) * 60;

            uint secondsNominator = (uint)Math.Truncate(10000000 * temp);
            uint secondsDenominator = 10000000;

            byte[] result = new byte[24];
            Array.Copy(BitConverter.GetBytes(degreesNumerator), 0, result, 0, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, result, 4, 4);

            Array.Copy(BitConverter.GetBytes(minutesNumerator), 0, result, 8, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, result, 12, 4);

            Array.Copy(BitConverter.GetBytes(secondsNominator), 0, result, 16, 4);
            Array.Copy(BitConverter.GetBytes(secondsDenominator), 0, result, 20, 4);
            return result;
        }
    }
}
