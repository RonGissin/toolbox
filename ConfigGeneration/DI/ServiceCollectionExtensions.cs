using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolBox.ConfigGeneration.Attributes;

namespace ToolBox.ConfigGeneration.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all configuration class that have <see cref="RuntimeConfigurationAttribute"/> defines them.
        /// Registration includes binding configuration to the class.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The app configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AutoRegisterConfigurations(this IServiceCollection services, IConfiguration configuration)
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
