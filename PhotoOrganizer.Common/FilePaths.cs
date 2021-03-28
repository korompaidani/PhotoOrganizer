namespace PhotoOrganizer.Common
{
    public static class FilePaths
    {
        public const string TempFilePostfix = @"_temp";
        public const string AppSettingsFile = @".\..\..\..\..\ProgramData\appSettings.json";
        public const string ErrorLogPath = @".\..\..\..\..\ProgramData\ErrorLogs";
        public const string ErrorLogFilePostfix = @"_errorlog.txt";
        public const string TestResources = @".\..\..\..\Resources\TestResources";
        public const string DefaultBackupFile = "_photoOrganizerBackup.xml";
        public const string DefaultBackupFolder = @".\..\..\..\Resources\Backup";
        public const string DefaultPicturePath = @".\..\..\..\Resources\Pictures\DefaultPicture.jpg";
        public const string MapLocation = @".\..\..\..\Resources\Web\OpenLayersMap.html";
    }
}
