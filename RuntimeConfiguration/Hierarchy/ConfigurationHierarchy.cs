using System.Text.Json.Nodes;

namespace ToolBox.RuntimeConfiguration.Hierarchy;

public class ConfigurationHierarchy
{
    public List<string> Hierarchy { get; set; }

    public List<JsonObject> Combinations { get; set; }
}
