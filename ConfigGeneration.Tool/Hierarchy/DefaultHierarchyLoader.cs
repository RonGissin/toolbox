using System.Text.Json;

namespace ToolBox.ConfigGeneration.Tool.Hierarchy;

public class DefaultHierarchyLoader : IHierarchyLoader
{
    public ConfigurationHierarchy LoadHierarchy(string hierarchyFilePath)
    {
        var hierarchyJson = File.ReadAllText(hierarchyFilePath);
        return JsonSerializer.Deserialize<ConfigurationHierarchy>(hierarchyJson);
    }
}
