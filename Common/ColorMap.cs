using System;
using System.Collections.Generic;

namespace PhotoOrganizer.Common
{
    [Flags]
    public enum ColorSign
    {
        Unmodified,
        Modified,
        Saved,
        Opened
    }

    public static class ColorMap
    {
        private static Dictionary<ColorSign, string> _map = new Dictionary<ColorSign, string>
        {
            { ColorSign.Unmodified, "#CCCCCC" },
            { ColorSign.Modified, "#EEEEEE" },
            { ColorSign.Saved, "#EEEEEE" },
            { ColorSign.Opened, "#EEEEEE" }
        };

        public static Dictionary<ColorSign, string> Map { get { return _map; } }
    }
}
