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
        SettingsError,
        UnKnownError,
        DataBaseError
    }

    public enum WarningTypes
    {
        DescriptionScriptWarning
    }

    public enum MetaProperty
    {
        Author = 40093,
        Title = 270,
        Comments = 40092,
        Keywords = 40094,
        DateTime = 36867,
        Latitude = 2,
        Longitude = 4,
        Desciprion = 37510
    }

    public enum HiddenMetaProperty
    {
        LatitudeDir = 1,
        LongitudeDir = 3,
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
