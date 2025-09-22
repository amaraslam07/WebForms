using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebForms.Http
{
    /// <summary>
    /// Factory for creating HTTP clients with authentication
    /// </summary>
    public class HttpClientFactory
    {
        /// <summary>
        /// Creates a new instance of HttpClientFactory
        /// </summary>
        public HttpClientFactory()
        {
        }

        /// <summary>
        /// Creates an HTTP client with Azure CLI authentication
        /// </summary>
        /// <param name="scope">The Azure AD scope for the token</param>
        /// <param name="baseAddress">Optional base address for the client</param>
        /// <returns>An HttpClient configured with Azure CLI authentication</returns>
        public HttpClient CreateAzureAuthenticatedClient(string scope, Uri baseAddress = null)
        {
            if (string.IsNullOrEmpty(scope))
                throw new ArgumentNullException(nameof(scope));

            // Create the delegating handler for Azure CLI authentication
            var handler = new AzureCliAuthenticationHandler(scope);
            
            // Create a new HTTP client with the handler
            var client = new HttpClient(handler);

            // Set the base address if provided
            if (baseAddress != null)
            {
                client.BaseAddress = baseAddress;
            }

            return client;
        }

        /// <summary>
        /// Creates an HTTP client with custom token authentication
        /// </summary>
        /// <param name="tokenProvider">A function that provides the authentication token</param>
        /// <param name="scheme">The authentication scheme (default: "Bearer")</param>
        /// <param name="baseAddress">Optional base address for the client</param>
        /// <returns>An HttpClient configured with custom token authentication</returns>
        public HttpClient CreateAuthenticatedClient(Func<Task<string>> tokenProvider, string scheme = "Bearer", Uri baseAddress = null)
        {
            if (tokenProvider == null)
                throw new ArgumentNullException(nameof(tokenProvider));

            // Create the delegating handler for custom token authentication
            var handler = new AuthenticationDelegatingHandler(tokenProvider, scheme);
            
            // Create a new HTTP client with the handler
            var client = new HttpClient(handler);

            // Set the base address if provided
            if (baseAddress != null)
            {
                client.BaseAddress = baseAddress;
            }

            return client;
        }
        
        /// <summary>
        /// Static method for backward compatibility
        /// </summary>
        public static HttpClient CreateAzureAuthenticatedClientStatic(string scope, Uri baseAddress = null)
        {
            if (string.IsNullOrEmpty(scope))
                throw new ArgumentNullException(nameof(scope));

            // Create the delegating handler for Azure CLI authentication
            var handler = new AzureCliAuthenticationHandler(scope);

            // Create an HTTP client with the handler
            var client = new HttpClient(handler);

            // Set the base address if provided
            if (baseAddress != null)
            {
                client.BaseAddress = baseAddress;
            }

            return client;
        }
    }
}