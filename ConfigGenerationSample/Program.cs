using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ToolBox.ConfigGeneration.DI;
using ToolBox.RuntimeConfigurationSample.src.configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("./src/configuration/generated/appsettings.development.eastus.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddConfigurations(context.Configuration);
    })
    .Build();

// Resolve services and run the application
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

// Example: Access configuration
var myConfig = services.GetRequiredService<IOptions<MyRuntimeConfiguration>>().Value;
Console.WriteLine($"Setting1: {myConfig.FirstProperty}");