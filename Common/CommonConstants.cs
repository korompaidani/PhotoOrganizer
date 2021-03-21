using System.Collections.Generic;

namespace PhotoOrganizer.Common
{
    public enum MessageDialogResult
    {
        Ok,
        Cancel,
        Yes,
        No,
        Save,
        SaveAll,
        Discard,
        DiscardAll,
        Extend,
        Overwrite
    }

    public enum ErrorTypes
    {
        DetailViewClosingError
    }

    public enum MetaProperty
    {
        Title = 40091,
        Comments = 40092,        
        Author = 40093,
        Keywords = 40094,
        Subject = 40095,
        Copyright = 33432,
        Software = 11,
        DateTime = 36867,
        Latitude = 2,
        Longitude = 22,
        Desciprion = 270,
    }

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
