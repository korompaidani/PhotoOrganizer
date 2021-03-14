using System.Collections.Generic;

namespace PhotoOrganizer.Common
{
    public enum Months
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public enum Days
    {        
    }

    public static class CommonContants
    {
        public static List<int> PageSizes = new List<int> { 50, 100, 500, 1000 };
    }

}
