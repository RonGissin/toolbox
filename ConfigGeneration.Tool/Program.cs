// Define command-line options with default values
using System.CommandLine;
using ToolBox.ConfigGeneration.Tool.AppSettings;
using ToolBox.ConfigGeneration.Tool.Generation;
using ToolBox.ConfigGeneration.Tool.Hierarchy;

var inputDirectoryOption = new Option<string>(
    "--inputDirectoryPath",
    () => "./src/configuration",
    "Path to the input directory"
);

var outputDirectoryOption = new Option<string>(
    "--outputDirectoryPath",
    () => "./src/configuration/generated",
    "Path to the output directory"
);

var hierarchyFileOption = new Option<string>(
    "--hierarchyFilePath",
    () => "./src/configuration/hierarchy.json",
    "Path to the hierarchy file"
);

var assembliesOption = new Option<string[]>(
    "--assembliesToSearch",
    () => Array.Empty<string>(),
    "Comma-separated list of assembly names to search (default: loaded assemblies)"
);

// Create the root command
var rootCommand = new RootCommand("GenerateConfigTool - A tool for generating configurations during builds.")
    {
        inputDirectoryOption,
        outputDirectoryOption,
        hierarchyFileOption,
        assembliesOption
    };

rootCommand.SetHandler((inputDir, outputDir, hierarchyPath, assembliesToSearch) =>
    {
        var hierarchyLoader = new DefaultHierarchyLoader();
        var configMapper = new ConfigurationMapper();
        var appSettingsWriter = new AppSettingsFileWriter();

        var configGenerator = new RuntimeConfigurationGenerator(
            hierarchyLoader,
            configMapper,
            appSettingsWriter);

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.FullName.EndsWith("ConfigGeneration.Tool"))
            .ToList();

        if (assembliesToSearch.Any())
        {
            loadedAssemblies = loadedAssemblies
                .Where(a => assembliesToSearch.Contains(a.FullName))
                .ToList();
        }

        var options = new RuntimeConfigurationGenerationOptions
        {
            InputDirectoryPath = inputDir,
            OutputDirectoryPath = outputDir,
            HierarchyFilePath = hierarchyPath,
            AssembliesToSearch = loadedAssemblies.ToList()
        };

        configGenerator.GenerateConfigurations(options);
    },
    inputDirectoryOption, outputDirectoryOption, hierarchyFileOption, assembliesOption);

await rootCommand.InvokeAsync(args);