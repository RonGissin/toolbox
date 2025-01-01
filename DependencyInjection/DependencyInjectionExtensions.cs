using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ToolBox.DependencyInjection;

/// <summary>
/// An extension class over <see cref="IServiceCollection"/>
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Verifies that all services registered in the <paramref name="services"/> can be resolved.
    /// This method must be called last after all registrations have been made, 
    /// registrations performed afterward will not be verified.
    /// </summary>
    /// <param name="services">The populated service collection.</param>
    /// <returns>The verified service provider.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a specific service cannot be resolved.
    /// </exception>
    public static IServiceCollection Verify(this IServiceCollection services)
    {
        // Build a temporary ServiceProvider for verification
        var temporaryProvider = services.BuildServiceProvider();

        var verifiableServices = services
            .Where(s => !s.ServiceType.IsGenericTypeDefinition)
            .Where(s => !typeof(IHost).IsAssignableFrom(s.ServiceType))
            .Where(s => !typeof(IHostEnvironment).IsAssignableFrom(s.ServiceType));

        foreach (var descriptor in verifiableServices)
        {
            try
            {
                var service = temporaryProvider.GetService(descriptor.ServiceType);

                if (service == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to resolve service {descriptor.ServiceType.FullName}.");
                }

                descriptor.CheckDependenciesLifetimeMismatch(services);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve service {descriptor.ServiceType.FullName}.", ex);
            }
        }

        return services;
    }
}

