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


```aiignore
<PropertyGroup>
        <ConfigGenVersion>1.0.9</ConfigGenVersion>
        <JsonConfigsDirectory>$(ProjectDir)configurations</JsonConfigsDirectory>
        <SettingsOutputDirectory>configurations/generated</SettingsOutputDirectory>
        <HierarchyFilePath>$(ProjectDir)configurations/hierarchy.json</HierarchyFilePath>
        <AssemblyToLoadPath>$(ProjectDir)bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).dll</AssemblyToLoadPath>
    </PropertyGroup>
    
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