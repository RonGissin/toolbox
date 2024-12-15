using System.ComponentModel.DataAnnotations;
using ToolBox.RuntimeConfiguration.AppSettings;
using ToolBox.RuntimeConfiguration.Generation;
using ToolBox.RuntimeConfiguration.Hierarchy;

namespace ToolBox.RuntimeConfiguration
{
    public class GenerateConfigurationsBuildTask : Microsoft.Build.Utilities.Task
    {
        public string HierarchyFilePath { get; set; } = "./src/configuration";
        public string InputDirectoryPath { get; set; } = "./src/configuration";
        public string OutputDirectoryPath { get; set; } = ".";

        public override bool Execute()
        {
            try
            {
                // Set up your dependencies
                var hierarchyLoader = new DefaultHierarchyLoader();
                var configurationMapper = new ConfigurationMapper();
                var appSettingsWriter = new AppSettingsFileWriter();

                var generator = new RuntimeConfigurationGenerator(hierarchyLoader, configurationMapper, appSettingsWriter);

                // Pass options to the generator
                var options = new RuntimeConfigurationGenerationOptions
                {
                    HierarchyFilePath = HierarchyFilePath,
                    InputDirectoryPath = InputDirectoryPath,
                    OutputDirectoryPath = OutputDirectoryPath,
                    AssembliesToSearch = AppDomain.CurrentDomain.GetAssemblies().ToList()
                };

                generator.GenerateConfigurations(options);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"Error generating runtime configurations: {ex.Message}");
                return false;
            }
        }
    }
}
