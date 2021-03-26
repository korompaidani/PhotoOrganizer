using PhotoOrganizer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PhotoOrganizer.FileHandler
{
    public class XmlWriterComponent
    {
        public async Task WriteXmlAsync(string filePath, Dictionary<string, List<Dictionary<string, Tuple<string, string>>>> data)
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

                    foreach (var firstLevel in data)
                    {
                        await writer.WriteStartElementAsync(null, firstLevel.Key, null);

                        foreach (var secondLevel in firstLevel.Value) 
                        {
                            foreach (var thirdLevel in secondLevel)
                            {
                                await writer.WriteStartElementAsync(null, thirdLevel.Key, null);
                                await writer.WriteAttributeStringAsync(null, "property", null, thirdLevel.Value.Item1);
                                await writer.WriteStringAsync(thirdLevel.Value.Item2.Replace("\0", string.Empty));
                                await writer.WriteEndElementAsync();
                            }
                        }

                        await writer.WriteEndElementAsync();
                    }

                    await writer.WriteEndElementAsync();
                    await writer.FlushAsync();
                }
            }
        }
    }
}
