namespace PhotoOrganizer.Common
{
    public static class FilePaths
    {
        public const string TempFilePostfix = @"_temp";
        public const string ProgramData = @".\..\..\..\..\ProgramData\";
        public const string DefaultBackupFolder = @".\..\..\..\..\ProgramData\Backup";
        public const string DbFullPath = @".\..\..\..\..\ProgramData\PhotoVersor.mdf";
        public const string AppSettingsFile = @".\..\..\..\..\ProgramData\appSettings.json";
        public const string ErrorLogPath = @".\..\..\..\..\ProgramData\ErrorLogs";
        public const string ErrorLogFilePostfix = @"_errorlog.txt";
        public const string TestResources = @".\..\..\..\Resources\TestResources";
        public const string DefaultBackupFile = "_photoOrganizerBackup.xml";
        public const string DefaultPicturePath = @".\..\..\..\Resources\Pictures\DefaultPicture.jpg";
        public const string MapLocation = @".\..\..\..\Resources\Web\OpenLayersMap.html";        
        public const string DataDirectory = "DataDirectory";
    }
}
