﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
        /// Verifies that all services registered in the <paramref name="services"/> can be resolved.
        /// This method must be called last after all registrations have been made, 
        /// registrations performed afterwards will not be verified.
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
                .Where(services => !typeof(IHostEnvironment).IsAssignableFrom(services.ServiceType));

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

                    CheckLifetimeMismatch(descriptor, services);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to resolve service {descriptor.ServiceType.FullName}.", ex);
                }
            }

            return services;
        }

        private static void CheckLifetimeMismatch(ServiceDescriptor descriptor, IServiceCollection services)
        {
            var implementationType = descriptor.ImplementationType;

            // If the service has an implementation factory or instance, skip it
            if (implementationType == null)
                return;

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
}

