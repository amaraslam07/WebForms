using System;
using System.Web.UI;
using Microsoft.Extensions.DependencyInjection;

namespace WebForms.DependencyInjection
{
    /// <summary>
    /// Helper class to make it easier to use dependency injection in WebForms
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// Gets a service from the dependency injection container
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <returns>The requested service or null if not found</returns>
        public static T GetService<T>()
        {
            return Global.GetService<T>();
        }

        /// <summary>
        /// Gets a required service from the dependency injection container
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <returns>The requested service</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service is not found</exception>
        public static T GetRequiredService<T>()
        {
            return Global.GetRequiredService<T>();
        }

        /// <summary>
        /// Extension method to resolve services in a Page
        /// </summary>
        /// <typeparam name="TService">The type of service to resolve</typeparam>
        /// <param name="page">The page instance</param>
        /// <returns>The resolved service</returns>
        public static TService Resolve<TService>(this Page page) where TService : class
        {
            return GetService<TService>();
        }

        /// <summary>
        /// Extension method to resolve required services in a Page
        /// </summary>
        /// <typeparam name="TService">The type of service to resolve</typeparam>
        /// <param name="page">The page instance</param>
        /// <returns>The resolved service</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service is not found</exception>
        public static TService ResolveRequired<TService>(this Page page) where TService : class
        {
            return GetRequiredService<TService>();
        }
    }
}