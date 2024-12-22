using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ToolBox.ConfigGeneration.Tool.Hierarchy;

public class ConfigurationHierarchy
{
    [JsonPropertyName("hierarchy")]
    public required List<string> Hierarchy { get; init; }

    [JsonPropertyName("combinations")]
    public required List<JsonObject> Combinations { get; init; }
}
