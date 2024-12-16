using System.Text.Json;
using System.Text.Json.Nodes;

namespace ToolBox.ConfigGeneration.Tool.AppSettings
{
    public class AppSettingsFileWriter : IAppSettingsFileWriter
    {
        public void WriteSettings(string outputDirectory, string fileName, JsonObject settings)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory cannot be null or empty.", nameof(outputDirectory));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                var filePath = Path.Combine(outputDirectory, fileName);

                var jsonContent = settings.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true // Format JSON with indentation for readability
                });

                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                throw; // Re-throw the exception for further handling if needed
            }
        }
    }
}
