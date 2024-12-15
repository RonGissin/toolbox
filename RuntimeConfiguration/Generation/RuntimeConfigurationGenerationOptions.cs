using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.RuntimeConfiguration.Generation
{
    public record RuntimeConfigurationGenerationOptions
    {
        public string InputDirectoryPath { get; init; }

        public string OutputDirectoryPath { get; init; }

        public string HierarchyFilePath { get; init; }

        public List<Assembly> AssembliesToSearch { get; init; }
    }
}
