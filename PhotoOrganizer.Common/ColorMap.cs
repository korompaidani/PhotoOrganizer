using System;
using System.Collections.Generic;

namespace PhotoOrganizer.Common
{
    [Flags]
    public enum ColorSign
    {
        Unmodified,
        Modified,
        Finalized,
        Opened,
        Checked
    }

    public static class ColorMap
    {
        private static Dictionary<ColorSign, string> _map = new Dictionary<ColorSign, string>
        {
            { ColorSign.Unmodified, "#CCCCCC" },
            { ColorSign.Modified, "#f6f7a6" },
            { ColorSign.Finalized, "#6dbd83" },
            { ColorSign.Opened, "#FFFFFF" },
            { ColorSign.Checked, "#0767b3" },
        };

        public static Dictionary<ColorSign, string> Map { get { return _map; } }
    }
}
