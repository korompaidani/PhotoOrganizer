using System;
using System.Text;

namespace PhotoOrganizer.MapTools
{
    public static class GoogleMapsStringExtension
    {
        private const string GoogleMapsUrl = "https://www.google.com/maps";

        public static string TryConvertUrlToCoordinate(this string inputUrl)
        {
            string result = "PARSE_ERROR";

            if (inputUrl == null)
            {
                return result;
            }

            if (inputUrl.Length != 0)
            {
                if (CheckItIsGoogleMapUrl(inputUrl))
                {
                    var coordinates = ParseGeoFromUrl(inputUrl);
                    var trimmedCoordinates = coordinates.Trim(new[] { '@', 'z', '/' });

                    if (CheckIfTextLessThanByteLong(trimmedCoordinates))
                    {
                        result = trimmedCoordinates;
                    }
                }
            }

            return result;
        }

        public static string ConvertCoordinateToGoogleMapUrl(this string coordinate)
        {
            var sb = new StringBuilder("https://www.google.hu/maps/");
            var coordinateSegments = coordinate.Split(',');

            if (coordinateSegments.Length == 3)
            {
                return sb.Append("@").Append(coordinate).Append("z").ToString();
            }
            else if(coordinateSegments.Length == 2)
            {
                return sb.Append("@").Append(coordinate).ToString();
            }
            else
            {
                return sb.ToString();
            }
        }

        private static bool CheckItIsGoogleMapUrl(string text)
        {
            if (text.Contains(GoogleMapsUrl))
            {
                return true;
            }

            return false;
        }

        private static string ParseGeoFromUrl(string urlText)
        {
            var url = new Uri(urlText);

            var segments = url.Segments;
            string coordinates = "";

            foreach (var segment in segments)
            {
                if (segment.StartsWith("@") && (segment.EndsWith("z") || segment.EndsWith("z/")))
                {
                    coordinates = segment;
                }
            }
            return coordinates;
        }

        private static bool CheckIfTextLessThanByteLong(string inputText)
        {
            if (inputText.Length > Byte.MaxValue)
            {
                return false;
            }

            return true;
        }
    }
}
