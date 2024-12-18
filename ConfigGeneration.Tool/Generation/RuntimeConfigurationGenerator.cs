using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToolBox.ConfigGeneration.Attributes;
using ToolBox.ConfigGeneration.Tool.AppSettings;
using ToolBox.ConfigGeneration.Tool.Hierarchy;

namespace ToolBox.ConfigGeneration.Tool.Generation;

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
        var combinations = BuildCombinations(options);

        WriteAppSettings(combinations, options.OutputDirectoryPath);
    }

    private void WriteAppSettings(Dictionary<string, Dictionary<string, object>> combinations, string outputPath)
    {
        foreach (var combination in combinations)
        {
            var appSettings = new JsonObject();

            foreach (var configuration in combination.Value)
            {
                var configName = configuration.Key;
                
                appSettings.Add(
                    configName,
                    JsonSerializer.SerializeToNode(combination.Value[configName]));
            }

            _appSettingsWriter.WriteSettings(outputPath, $"appsettings.{combination.Key}", appSettings);
        }
    }

    private Dictionary<string, Dictionary<string, object>> BuildCombinations(RuntimeConfigurationGenerationOptions options)
    {
        Dictionary<string, Dictionary<string, object>> combinations = new();

        var hierarchy = _hierarchyLoader.LoadHierarchy(options.HierarchyFilePath);

        var configurationTypes = GetConfigurationTypes(options.AssembliesToSearch);

        foreach (var combination in hierarchy.Combinations)
        {
            Dictionary<string, object> configurations = new();

            foreach (var type in configurationTypes)
            {
                var configName = type.Name;
                var configPath = Path.Combine(options.InputDirectoryPath, $"{configName}.json");
                var jsonObject = JsonNode.Parse(File.ReadAllText(configPath)).AsObject();

                var configuration = _configurationMapper.MapFromJson(type, jsonObject, combination, hierarchy);

                configurations.Add(configName, configuration);
            }

            combinations.Add(GetCombinationIdentifier(combination), configurations);
        }

        return combinations;
    }

    private string GetCombinationIdentifier(JsonObject combination)
    {
        StringBuilder sb = new();

        foreach (var (key, value) in combination)
        {
            sb.Append($"{value}.");
        }

        sb.Append("json");

        return sb.ToString();
    }

    private List<Type> GetConfigurationTypes(List<Assembly> assemblies)
    {
        List<Type> configurationTypes = new();

        foreach (var assembly in assemblies)
        {
            var types = assembly
                .GetTypes()
                .Where(
                    t => t.GetCustomAttributes()
                    .Any(a => a.GetType().Name.EndsWith("RuntimeConfigurationAttribute")));

            configurationTypes.AddRange(types);
        }

        return configurationTypes;
    }

    private readonly IConfigurationMapper _configurationMapper;
    private readonly IAppSettingsFileWriter _appSettingsWriter;
    private readonly IHierarchyLoader _hierarchyLoader;
}
