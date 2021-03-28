using PhotoOrganizer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PhotoOrganizer.FileHandler
{
    public static class ErrorMessageWriter
    {
        public static void WriteErrorMessagesToFile(List<KeyValuePair<ErrorTypes, string>> errorMessages)
        {
            var dateTime = DateTime.Now;
            var cultureInfo = CultureInfo.InvariantCulture;
            var datePrefix = dateTime.ToString("yyyyMMddhhmmss", cultureInfo);

            var fileName = new StringBuilder(datePrefix).Append(FilePaths.ErrorLogFilePostfix);
            var filePath = Path.Combine(FilePaths.ErrorLogPath, fileName.ToString());

            using (var sw = new StreamWriter(filePath))
            {
                foreach(var message in errorMessages)
                {
                    sw.WriteLine("=========================================================================================");
                    sw.WriteLine($"***************************** {message.Key} *****************************");
                    sw.WriteLine("=========================================================================================");
                    sw.WriteLine($"{message.Value}");
                    sw.WriteLine("#########################################################################################");
                }
            }
        }
    }
}
