using System.Reflection;

namespace ToolBox.ConfigGeneration.Tool.Generation
{
    public record RuntimeConfigurationGenerationOptions
    {
        public required string InputDirectoryPath { get; init; }

        public required string OutputDirectoryPath { get; init; }

        public required string HierarchyFilePath { get; init; }

        public required List<Assembly> AssembliesToSearch { get; init; }
    }
}
