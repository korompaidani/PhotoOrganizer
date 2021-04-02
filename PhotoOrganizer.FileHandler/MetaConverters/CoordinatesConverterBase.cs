using PhotoOrganizer.Common;
using System;
using System.Drawing;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class CoordinatesConverterBase : ConverterBase
    {
        protected MetaProperty DirMetaType;
        protected char PositiveDirection;
        protected char NegativeDirection;
        protected byte CoordinateIndex;

        public override string ConvertMetaToProperty(byte[] data)
        {
            return string.Empty;
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
            int degrees = (int)Math.Truncate(temp);
            temp = (temp - degrees) * 60;

            int minutes = (int)Math.Truncate(temp);
            temp = (temp - minutes) * 60;

            int secondsNominator = (int)Math.Truncate(10000000 * temp);
            int secondsDenoninator = 10000000;

            byte[] result = new byte[24];
            Array.Copy(BitConverter.GetBytes(degrees), 0, result, 0, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, result, 4, 4);
            Array.Copy(BitConverter.GetBytes(minutes), 0, result, 8, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, result, 12, 4);
            Array.Copy(BitConverter.GetBytes(secondsNominator), 0, result, 16, 4);
            Array.Copy(BitConverter.GetBytes(secondsDenoninator), 0, result, 20, 4);
            return result;
        }
    }
}
