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
    }
}
