using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DependencyInjection
{
    /// <summary>
    /// An extension class over <see cref="IServiceCollection"/>
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers a decorator for the specified service type.
        /// The decorated registration lifetime will be defined by the decoratee's defined lifetime.
        /// Multiple decorators can be added for the same <typeparamref name="TService"/> - they will be applied in the order they are added.
        /// </summary>
        /// <typeparam name="TService">The decorated service.</typeparam>
        /// <typeparam name="TDecorator">The decorator service.</typeparam>
        /// <param name="services"></param>
        /// <returns>The service collection.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TService"/> is not yet registered to the service collection.</exception>
        public static IServiceCollection AddDecorator<TService, TDecorator>(this IServiceCollection services)
            where TService : class
            where TDecorator : class, TService
        {
            // Find the original service descriptor
            var decorateeDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            if (decorateeDescriptor == null)
            {
                throw new InvalidOperationException($"Service of type {typeof(TService).FullName} not found in the service collection.");
            }

            // Remove the original descriptor
            services.Remove(decorateeDescriptor);

            // Add the decorator
            services.Add(new ServiceDescriptor(typeof(TService), provider =>
            {
                // Resolve the original service
                var originalImplementation = (TService)decorateeDescriptor.ImplementationFactory?.Invoke(provider)
                                             ?? provider.GetRequiredService(decorateeDescriptor.ImplementationType);

                // Create and return the decorator, passing the original implementation
                return ActivatorUtilities.CreateInstance<TDecorator>(provider, originalImplementation);
            }, decorateeDescriptor.Lifetime));

            return services;
        }

        /// <summary>
        /// Verifies that all services registered in the <paramref name="serviceProvider"/> can be resolved.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The verified service provider.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a specific service cannot be resolved.
        /// </exception>
        public static IServiceProvider Verify(this IServiceProvider serviceProvider)
        {
            // Iterate over all registered services
            var serviceDescriptors = serviceProvider.GetRequiredService<IEnumerable<ServiceDescriptor>>();
            foreach (var serviceDescriptor in serviceDescriptors)
            {
                try
                {
                    // Skip open generic types and instances directly provided
                    if (serviceDescriptor.ServiceType.IsGenericTypeDefinition || serviceDescriptor.ImplementationInstance != null)
                    {
                        continue;
                    }

                    // Attempt to resolve the service
                    var service = serviceProvider.GetService(serviceDescriptor.ServiceType);

                    // If the service is null, it may indicate an issue
                    if (service == null)
                    {
                        throw new InvalidOperationException($"Failed to resolve service {serviceDescriptor.ServiceType.FullName}");
                    }
                }
                catch (Exception ex)
                {
                    // Log or rethrow the exception for misconfigured services
                    throw new InvalidOperationException($"Failed to resolve service {serviceDescriptor.ServiceType.FullName}", ex);
                }
            }

            return serviceProvider;
        }
    }
}

