using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebForms.DependencyInjection
{
    /// <summary>
    /// Extensions for ServiceCollection to support .NET Framework 4.7.2
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Builds a ServiceProvider from the ServiceCollection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>A service provider containing all the registered services</returns>
        public static IServiceProvider BuildServiceProvider(this IServiceCollection services)
        {
            return new DefaultServiceProvider(services);
        }
        
        /// <summary>
        /// Simple implementation of IServiceProvider for .NET Framework 4.7.2
        /// </summary>
        private class DefaultServiceProvider : IServiceProvider
        {
            private readonly IServiceCollection _services;
            
            public DefaultServiceProvider(IServiceCollection services)
            {
                _services = services ?? throw new ArgumentNullException(nameof(services));
            }
            
            public object GetService(Type serviceType)
            {
                if (serviceType == null)
                    throw new ArgumentNullException(nameof(serviceType));
                
                // Find the service descriptor for the requested type
                var descriptor = FindServiceDescriptor(serviceType);
                if (descriptor == null)
                    return null;
                
                // Create the service based on the descriptor's lifetime
                return CreateService(descriptor);
            }
            
            private ServiceDescriptor FindServiceDescriptor(Type serviceType)
            {
                // Look for an exact match first
                foreach (var descriptor in _services)
                {
                    if (descriptor.ServiceType == serviceType)
                        return descriptor;
                }
                
                return null;
            }
            
            private object CreateService(ServiceDescriptor descriptor)
            {
                if (descriptor.ImplementationInstance != null)
                    return descriptor.ImplementationInstance;
                
                if (descriptor.ImplementationFactory != null)
                    return descriptor.ImplementationFactory(this);
                
                if (descriptor.ImplementationType != null)
                    return Activator.CreateInstance(descriptor.ImplementationType);
                
                return null;
            }
        }
    }
}