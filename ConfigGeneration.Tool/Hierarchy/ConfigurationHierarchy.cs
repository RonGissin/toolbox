using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ToolBox.ConfigGeneration.Tool.Hierarchy;

public class ConfigurationHierarchy
{
    [JsonPropertyName("hierarchy")]
    public List<string> Hierarchy { get; set; }

    [JsonPropertyName("combinations")]
    public List<JsonObject> Combinations { get; set; }
}
