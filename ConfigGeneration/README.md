# ToolBox.ConfigGeneration #

ConfigGeneration is a library which helps to streamline the interaction with runtime configurations.

Instead of maintaining an enormous `appsettings.json` file or registering your IOptions<T> to your service collection again and again, this library takes care of automating this for you.

## Introduction

When using `ToolBox.ConfigGeneration`,
you declare your configuration in a Domain Driven Design manner.\
Practically, this means that instead of maintaining a *single* `appsettings` file for *all* configurations - you manage each configuration.

For example, say you have a redis configuration:


```aiignore
public class RedisConfiguration {
    public string RedisUri { get; set; }
}
```

and a postgres db configuration:

```aiignore
public class PostgresConfiguration {
    public string DbUri { get; set; }
}
```

For each of those you will define a json file **with an equivalent name**.\
Below is the example for the `RedisConfiguration.json`:

`RedisConfiguration`
```aiignore
{
    "RedisUri": {
        "development": {
            "eastus": "https://...",
            "westeurope": "https://..."
        },
        "production": {
            "eastus": "https://...",
            "westeurope": "https://..."
        }
    }
}
```

**Note the inner hierarchy defined under the `RedisUri` property.**

**You have full control** over the hierarchy of your configurations.

See below section for more info.

## Hierarchy file

In your executable project, you define a single hierarchy file.

Your hierarchy will be defined by the various deployment environments, regions / locations or any other granularity you might need.

The hierarchy file is a `.json` file and must have two key-value pairs:

1. `hierarchy` (array) - a list of nested hierarchies, by order.
2. `combinations` (array) - a list of all combinations that must have their own dedicated settings.

For example, the hierarchy file suitable to fit the above `RedisConfiguration.json` is:

`hierarchy.json`
```aiignore
{
    "hierarchy": ["environment", "region"],
    "combinations": [
        {
            "environment": "development",
            "region": "eastus"
        },
        {
            "environment": "development",
            "region": "westeurope"
        },
        {
            "environment": "production",
            "region": "eastus"
        },
        {
            "environment": "production",
            "region": "westeurope"
        },
    ]
}
```

As you can see, there are 4 combinations defined in the above file.\
This means there is a total of 4 distinct environments - each deserving of its own set of configuration values.

Read below section to learn how to **generate the configurations**
## Configuration generation

To take care of generating the appropriate appsettings files out of your configurations, you can add the following code to your executable's `.csproj` file.


```aiignore
    <PropertyGroup>
        <ConfigGenVersion>1.0.9</ConfigGenVersion>
        <JsonConfigsDirectory>$(ProjectDir)configurations</JsonConfigsDirectory>
        <SettingsOutputDirectory>configurations/generated</SettingsOutputDirectory>
        <HierarchyFilePath>$(ProjectDir)configurations/hierarchy.json</HierarchyFilePath>
        <AssemblyToLoadPath>$(ProjectDir)bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).dll</AssemblyToLoadPath>
    </PropertyGroup>
    
    <ItemGroup>
        <PackageReference Include="ToolBox.ConfigGeneration" Version="$(ConfigGenVersion)" />
    </ItemGroup>

    <Target Name="RunConfigTool" AfterTargets="Build">
        <Message Importance="High" Text="Checking if tool manifest exists..." />
        <Exec Command="dotnet new tool-manifest" ContinueOnError="true" />
        <Message Importance="High" Text="Ensuring toolbox-config-gen is installed..." />
        <Exec Command="dotnet tool install --local ConfigGeneration.Tool --version $(ConfigGenVersion)" ContinueOnError="true" />
        <Message Importance="High" Text="Running toolbox-config-gen to generate configuration..." />
        <Exec Command="
            dotnet tool run toolbox-config-gen --jsonConfigsDirectory $(JsonConfigsDirectory) --settingsOutputDirectory $(ProjectDir)$(SettingsOutputDirectory) --hierarchyFilePath $(HierarchyFilePath) --assemblyToLoadPath $(AssemblyToLoadPath)" />
    </Target>
    <Target Name="CopyGeneratedJson" AfterTargets="RunConfigTool">
        <Message Importance="High" Text="Copying the generated settings files..." />
        <ItemGroup>
            <GeneratedJsonFiles Include="$(ProjectDir)$(SettingsOutputDirectory)/**/*.json" />
        </ItemGroup>

        <!-- Copy files to the output directory -->
        <Copy SourceFiles="@(GeneratedJsonFiles)" DestinationFolder="$(OutputPath)$(SettingsOutputDirectory)" />
    </Target>
```

The code you see does the following:

1. Declares some variables to be used in the following target.
   1. `ConfigGenVersion` - The version of `ConfigGeneration.Tool` to use for the generation process.
   2. `JsonConfigsDirectory` - The root directory in which to search for configuration files (`.json` files).
   3. `SettingsOutputDirectory` - The directory in which to generate the `appsettings` files.
   4. `HierarchyFilePath` - The path to your `hierarchy.json` file.
   5. `AssemblyToLoadPath` - The path to your executable assembly (the one which is your app's entrypoint).

2. Adds a package reference to the `ToolBox.ConfigGeneration` package.\
   Note: The version of the package is defined by the `ConfigGenVersion` variable, and must match between the tool and the library.

3. Declares a target which:
   1. Creates a new tool manifest for your project (if not exists).
   2. Installs the `ConfigGeneration.Tool` dotnet tool locally in your project.
   3. Runs the `toolbox-config-gen` command with your configured msbuild variables to generate the settings files.

4. Declares another target which copies the generated settings files to your output directory.\
This is critical to enable your application to read them during runtime.


Once you add this to your `.csproj`, build the project and validate that the generation succeeded.

## Using the generated files in your application

Now, say you have accomplished the following steps:

Created an app --> added the tool using the provided targets --> successfully built the project.

The next step is to enable your application to read the generated configurations.

See the example Program.cs file below:

```aiignore
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Test.configurations;
using ToolBox.ConfigGeneration.DI;

Environment.SetEnvironmentVariable("RedisConfiguration:RedisUri", "SomeOverrideValue");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add the generated file - can use environment variables to point to the correct one per combination.
        config.AddJsonFile("./configurations/generated/appsettings.development.eastus.json", optional: false, reloadOnChange: true);
        
        // Allow overriding specific values using environment variables like the one above !
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        // Automatically binds all configurations with the `RuntimeConfigurationAttribute` to corresponding json values.
        services.AutoRegisterConfigurations(context.Configuration);
    })
    .Build();

// Resolve services and run the application
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

// Example: Access configuration
var myConfig = services.GetRequiredService<IOptions<RedisConfiguration>>().Value;

Console.WriteLine($"RedisConfiguration is {JsonSerializer.Serialize(myConfig)}.");
```