using PhotoOrganizer.Common;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PhotoOrganizer.FileHandler
{
    public class XmlWriterComponent
    {
        public async Task WriteXmlAsync(string filePath)
        {
            var dateTime = DateTime.Now;
            var cultureInfo = CultureInfo.InvariantCulture;
            var datePrefix = dateTime.ToString("yyyyMMddhhmmss", cultureInfo);

            var fileName = new StringBuilder(datePrefix);
            fileName.Append(FilePaths.DefaultBackupFile);
            filePath = Path.Combine(filePath, fileName.ToString());

            using(var fs = File.Create(filePath))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Async = true;

                using (XmlWriter writer = XmlWriter.Create(fs, settings))
                {
                    await writer.WriteStartElementAsync("pf", "root", "http://ns");
                    await writer.WriteStartElementAsync(null, "sub", null);
                    await writer.WriteAttributeStringAsync(null, "att", null, "val");
                    await writer.WriteStringAsync("text");
                    await writer.WriteEndElementAsync();
                    await writer.WriteProcessingInstructionAsync("pName", "pValue");
                    await writer.WriteCommentAsync("cValue");
                    await writer.WriteCDataAsync("cdata value");
                    await writer.WriteEndElementAsync();
                    await writer.FlushAsync();
                }
            }
        }
    }
}
