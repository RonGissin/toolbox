using System.Text.Json;
using System.Text.Json.Nodes;
using ToolBox.Safety;

namespace ToolBox.ConfigGeneration.Tool.AppSettings
{
    public class AppSettingsFileWriter : IAppSettingsFileWriter
    {
        public void WriteSettings(string outputDirectory, string fileName, JsonObject settings)
        {
            Safe.ThrowIfNullOrEmpty(outputDirectory);
            Safe.ThrowIfNullOrEmpty(fileName);
            Safe.ThrowIfNull(settings);

            // Ensure the output directory exists
            Directory.CreateDirectory(outputDirectory);

            var filePath = Path.Combine(outputDirectory, fileName);

            var jsonContent = settings.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true // Format JSON with indentation for readability
            });

            File.WriteAllText(filePath, jsonContent);
        }
    }
}
