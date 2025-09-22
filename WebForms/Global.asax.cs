using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using WebForms.Http;
using WebForms.Services;
using System.Configuration;
using WebForms.DependencyInjection;

namespace WebForms
{
    public class Global : HttpApplication
    {
        // Static ServiceProvider that will be accessible throughout the application
        public static IServiceProvider ServiceProvider { get; private set; }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Configure dependency injection
            ConfigureServices();
        }
        
        private void ConfigureServices()
        {
            // Create a new service collection
            var services = new ServiceCollection();
            
            // Register your custom HttpClientFactory as a singleton
            services.AddSingleton<HttpClientFactory>(new HttpClientFactory());
            
            // Register configuration settings as service factory methods
            services.AddTransient(provider => 
                ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://api.example.com");
                
            services.AddTransient(provider => 
                ConfigurationManager.AppSettings["ApiScope"] ?? "api://example-api/.default");
            
            // Register ApiService with factory pattern to resolve constructor dependencies
            services.AddScoped<ApiService>(provider => {
                var httpClientFactory = provider.GetService<HttpClientFactory>();
                var apiBaseUrl = provider.GetService<string>();
                var apiScope = provider.GetService<string>();
                
                return new ApiService(httpClientFactory, apiBaseUrl, apiScope);
            });
            
            // Build the service provider using our custom extension method
            ServiceProvider = services.BuildServiceProvider();
        }
        
        // Helper method to get services
        public static T GetService<T>()
        {
            return (T)ServiceProvider.GetService(typeof(T));
        }
        
        // Helper method to get required services (throws if service not found)
        public static T GetRequiredService<T>()
        {
            var service = GetService<T>();
            if (service == null)
            {
                throw new InvalidOperationException($"Service of type {typeof(T).FullName} not found.");
            }
            return service;
        }
    }
}