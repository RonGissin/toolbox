// Define command-line options with default values
using System.CommandLine;
using System.Reflection;
using ToolBox.ConfigGeneration.Tool.AppSettings;
using ToolBox.ConfigGeneration.Tool.Generation;
using ToolBox.ConfigGeneration.Tool.Hierarchy;

var jsonConfigsDirectory = new Option<string>(
    "--jsonConfigsDirectory",
    () => "./src/configuration",
    "Path to the directory containing JSON configuration files."
);

var outputDirectoryOption = new Option<string>(
    "--settingsOutputDirectory",
    () => "./src/configuration/generated",
    "Path to the output directory for the generated settings files."
);

var hierarchyFileOption = new Option<string>(
    "--hierarchyFilePath",
    () => "./src/configuration/hierarchy.json",
    "Path to the hierarchy file"
);

var assemblyOption = new Option<string>(
    "--assemblyToLoadPath",
    "Path to the assembly to load and search."
);

// Create the root command
var rootCommand = new RootCommand("GenerateConfigTool - A tool for generating configurations during builds.")
    {
        jsonConfigsDirectory,
        outputDirectoryOption,
        hierarchyFileOption,
        assemblyOption
    };

rootCommand.SetHandler((inputDir, outputDir, hierarchyPath, assemblyToLoad) =>
    {
        var hierarchyLoader = new DefaultHierarchyLoader();
        var configMapper = new ConfigurationMapper();
        var appSettingsWriter = new AppSettingsFileWriter();

        var configGenerator = new RuntimeConfigurationGenerator(
            hierarchyLoader,
            configMapper,
            appSettingsWriter);

        var execAssembly = Assembly.LoadFrom(assemblyToLoad);
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Union(new[] { execAssembly })
            .ToList();
        
        var options = new RuntimeConfigurationGenerationOptions
        {
            InputDirectoryPath = inputDir,
            OutputDirectoryPath = outputDir,
            HierarchyFilePath = hierarchyPath,
            AssembliesToSearch = loadedAssemblies.ToList()
        };

        configGenerator.GenerateConfigurations(options);
    },
    jsonConfigsDirectory, outputDirectoryOption, hierarchyFileOption, assemblyOption);

await rootCommand.InvokeAsync(args);