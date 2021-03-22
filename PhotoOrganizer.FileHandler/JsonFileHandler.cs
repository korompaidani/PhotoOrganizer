using Newtonsoft.Json;
using PhotoOrganizer.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoOrganizer.FileHandler
{
    public class JsonFileHandler<T>
    {
        public async Task WriteModelToFileAsync(T model)
        {
            var file = FilePaths.AppSettingsFile;
            var jsonContent = SerializeJson(model);
            await WriteFileAsync(file, jsonContent);
        }

        public async Task<T> ReadModelFromFileAsync()
        {
            var file = FilePaths.AppSettingsFile;
            var json = await ReadFileAsync(file);
            return DeserializeJson(json);
        }

        public T InitialReadModelFromFile()
        {
            var file = FilePaths.AppSettingsFile;
            var json = InitialReadFile(file);
            return DeserializeJson(json);
        }

        private string SerializeJson(T model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }

        private T DeserializeJson(string file)
        {
            return JsonConvert.DeserializeObject<T>(file);
        }

        private async Task<string> ReadFileAsync(string file)
        {
            string result = String.Empty;
            using (var reader = File.OpenText(file))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task WriteFileAsync(string file, string jsonContent)
        {
            using (StreamWriter outputFile = new StreamWriter(file))
            {
                await outputFile.WriteAsync(jsonContent);
            }
        }

        private string InitialReadFile(string file)
        {
            string result = String.Empty;
            using (var reader = File.OpenText(file))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
