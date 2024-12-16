using System.Text.Json.Nodes;

namespace ToolBox.ConfigGeneration.Tool.Hierarchy;

public class ConfigurationHierarchy
{
    public List<string> Hierarchy { get; set; }

    public List<JsonObject> Combinations { get; set; }
}
