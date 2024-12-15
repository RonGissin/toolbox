using System.Reflection;
using System.Text.Json.Nodes;
using ToolBox.RuntimeConfiguration.AppSettings;
using ToolBox.RuntimeConfiguration.Attributes;
using ToolBox.RuntimeConfiguration.Hierarchy;

namespace ToolBox.RuntimeConfiguration.Generation;

public class RuntimeConfigurationGenerator : IRuntimeConfigurationGenerator
{
    public RuntimeConfigurationGenerator(
        IHierarchyLoader hierarchyLoader,
        IConfigurationMapper configurationMapper,
        IAppSettingsFileWriter appSettingsWriter)
    {
        _configurationMapper = configurationMapper ?? throw new ArgumentNullException(nameof(configurationMapper));
        _hierarchyLoader = hierarchyLoader ?? throw new ArgumentNullException(nameof(appSettingsWriter));
        _appSettingsWriter = appSettingsWriter ?? throw new ArgumentNullException(nameof(appSettingsWriter));
    }

    public void GenerateConfigurations(RuntimeConfigurationGenerationOptions options)
    {
        var hierarchy = _hierarchyLoader.LoadHierarchy(options.HierarchyFilePath);

        var configurationTypes = GetConfigurationTypes(options.AssembliesToSearch);

        foreach (var combination in hierarchy.Combinations)
        {
            List<object> configurations = new ();

            foreach (var type in configurationTypes)
            {
                var configName = type.Name;
                var configPath = Path.Combine(options.InputDirectoryPath, $"{configName}.json");
                var jsonObject = JsonNode.Parse(File.ReadAllText(configPath)).AsObject();

                var configuration = _configurationMapper.MapFromJson(type, jsonObject, combination, hierarchy);

                configurations.Add(configuration);
            }
        }
    }
        

    private List<Type> GetConfigurationTypes(List<Assembly> assemblies)
    {
        List<Type> configurationTypes = new();

        foreach (var assembly in assemblies)
        {
            var types = assembly
                .GetTypes()
                .Where(t => t.IsDefined(typeof(RuntimeConfigurationAttribute)));

            configurationTypes.AddRange(types);
        }

        return configurationTypes;
    }

    private readonly IConfigurationMapper _configurationMapper;
    private readonly IAppSettingsFileWriter _appSettingsWriter;
    private readonly IHierarchyLoader _hierarchyLoader;
}
