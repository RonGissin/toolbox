using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolBox.ConfigGeneration.Attributes;

namespace ToolBox.ConfigGeneration.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var configTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<RuntimeConfigurationAttribute>() != null)
                .Where(t => !t.IsAbstract && t.IsClass);

            foreach (var configType in configTypes)
            {
                // Bind configuration to the class
                var sectionName = configType.Name; // Or customize section name logic if needed
                var configSection = configuration.GetSection(sectionName);

                // Use reflection to call services.Configure<T>()
                var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Configure" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(configType);

                configureMethod.Invoke(null, new object[] { services, configSection });
            }

            return services;
        }
    }
}
