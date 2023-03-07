using System;
using System.IO;

namespace PhotoOrganizer.Common
{
    public static class FilePaths
    {
        public static string DocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static string TempFilePostfix = @"_temp";
        public static string ProgramData = Path.Combine(DocumentFolder, "PhotoVersorData"); /*@".\..\..\..\..\ProgramData\";*/
        public static string ThumbnailsRootFolder = Path.Combine(ProgramData, "Thumbnails"); /*@".\..\..\..\..\ProgramData\Thumbnails";*/
        public static string DefaultBackupFolder = Path.Combine(ProgramData, "Backup"); /*@".\..\..\..\..\ProgramData\Backup";*/
        public static string DbFullPath = Path.Combine(ProgramData, "PhotoVersor.mdf"); /*@".\..\..\..\..\ProgramData\PhotoVersor.mdf";*/
        public static string AppSettingsFile = Path.Combine(ProgramData, "appSettings.json"); /*@".\..\..\..\..\ProgramData\appSettings.json";*/
        public static string ErrorLogPath = Path.Combine(ProgramData, "ErrorLogs"); /*@".\..\..\..\..\ProgramData\ErrorLogs";*/
        public static string ErrorLogFilePostfix = @"_errorlog.txt";
        public static string TestResources = @".\..\..\..\Resources\TestResources";
        public static string DefaultBackupFile = "_photoOrganizerBackup.xml";
        public static string DefaultPicturePath = @".\..\..\..\Resources\Pictures\DefaultPicture.jpg";
        public static string MapLocation = ResolveLocationForMap();
        public static string DataDirectory = "DataDirectory";
        public static string ExplorerExe = "explorer.exe";

        private static string ResolveLocationForMap()
        {
            string resultPath = Path.Combine(ProgramData, "Map", "OpenLayersMap.html");

            if (!File.Exists(resultPath))
            {
                resultPath = @".\..\..\..\Resources\Web\OpenLayersMap.html";
            }

            return resultPath;
        }
    }
}
