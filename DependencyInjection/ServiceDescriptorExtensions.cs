using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.DependencyInjection;

/// <summary>
/// Extension methods on <see cref="ServiceDescriptor"/>
/// </summary>
public static class ServiceDescriptorExtensions
{
    /// <summary>
    /// Checks if the lifetimes of the dependencies are compatible with the lifetime of the service.
    /// </summary>
    /// <param name="descriptor">The service to check.</param>
    /// <param name="services">A collection of services. Must contain all the descriptor's dependencies.</param>
    /// <returns>The descriptor.</returns>
    public static ServiceDescriptor CheckDependenciesLifetimeMismatch(this ServiceDescriptor descriptor, IServiceCollection services)
    {
        var implementationType = descriptor.ImplementationType;

        // If the service has an implementation factory or instance, skip it
        if (implementationType == null)
        {
            return descriptor;
        }

        // Get all constructor dependencies
        var constructors = implementationType.GetConstructors();
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();

            foreach (var parameter in parameters)
            {
                var dependencyDescriptor = services.FirstOrDefault(d => d.ServiceType == parameter.ParameterType);

                if (dependencyDescriptor == null)
                {
                    continue;
                }

                ValidateLifetimeRules(descriptor, dependencyDescriptor);
            }
        }

        return descriptor;
    }

    private static void ValidateLifetimeRules(ServiceDescriptor consumer, ServiceDescriptor dependency)
    {
        var valid = consumer.Lifetime switch
        {
            ServiceLifetime.Transient => true,
            ServiceLifetime.Scoped => dependency.Lifetime != ServiceLifetime.Transient,
            ServiceLifetime.Singleton => dependency.Lifetime == ServiceLifetime.Singleton,
            _ => false
        };

        if (!valid)
        {
            throw new InvalidOperationException(
                $"Lifetime mismatch detected: {consumer.ServiceType.FullName} ({consumer.Lifetime}) depends on {dependency.ServiceType.FullName} ({dependency.Lifetime}).");
        }
    }
}
