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
        DetailViewClosingError,
        MetaWritingError,
        BackupError,
        InitializationError,
        BulkSetAttributError,
        DirectoryReaderError,
        CacheError,
        MetaReadError,
        SettingsError
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
        ExiflibTitle = 100269, //DocumentName
        ExiflibComments = 237510, //UserComment  
        ExiflibAuthor = 140093, //WindowsAuthor
        ExiflibKeywords = 140094, //WindowsKeywords
        ExiflibSubject = 140095, //WindowsSubject
        ExiflibCopyright = 133432,
        ExiflibSoftware = 100305,
        ExiflibDateTime = 236867, //DateTimeOriginal
        ExiflibLatitude = 300002,
        ExiflibLongitude = 300004,
        ExiflibDesciprion = 100270 //ImageDescription
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
        public static List<string> Languages = new List<string> { "en-US", "hu-HU" };
    }

}
