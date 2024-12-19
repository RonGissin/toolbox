using System.Text.Json;
using ToolBox.Safety;

namespace ToolBox.ConfigGeneration.Tool.Hierarchy;

public class DefaultHierarchyLoader : IHierarchyLoader
{
    public ConfigurationHierarchy LoadHierarchy(string hierarchyFilePath)
    {
        Safe.ThrowIfNullOrEmpty(hierarchyFilePath);
        
        var hierarchyJson = File.ReadAllText(hierarchyFilePath);
        return JsonSerializer.Deserialize<ConfigurationHierarchy>(hierarchyJson);
    }
}
