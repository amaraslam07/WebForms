using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebForms.Http;

namespace WebForms.Services
{
    /// <summary>
    /// Example service that makes authenticated API calls
    /// </summary>
    public class ApiService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="ApiService"/>
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory from DI</param>
        /// <param name="apiBaseUrl">The base URL of the API</param>
        /// <param name="scope">The Azure AD scope for authentication</param>
        public ApiService(HttpClientFactory httpClientFactory, string apiBaseUrl, string scope)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));
                
            if (string.IsNullOrEmpty(apiBaseUrl))
                throw new ArgumentNullException(nameof(apiBaseUrl));
            
            if (string.IsNullOrEmpty(scope))
                throw new ArgumentNullException(nameof(scope));

            _apiBaseUrl = apiBaseUrl;
            
            // Create the HTTP client with Azure CLI authentication
            _httpClient = httpClientFactory.CreateAzureAuthenticatedClient(
                scope, 
                new Uri(apiBaseUrl));
        }
        
        /// <summary>
        /// Legacy constructor for backward compatibility
        /// </summary>
        public ApiService(string apiBaseUrl, string scope)
        {
            if (string.IsNullOrEmpty(apiBaseUrl))
                throw new ArgumentNullException(nameof(apiBaseUrl));
            
            if (string.IsNullOrEmpty(scope))
                throw new ArgumentNullException(nameof(scope));

            _apiBaseUrl = apiBaseUrl;
            
            // Create the HTTP client with Azure CLI authentication
            _httpClient = HttpClientFactory.CreateAzureAuthenticatedClientStatic(
                scope, 
                new Uri(apiBaseUrl));
        }

        /// <summary>
        /// Gets data from a protected API endpoint
        /// </summary>
        /// <param name="endpoint">The API endpoint relative to the base URL</param>
        /// <returns>The API response as a string</returns>
        public async Task<string> GetDataAsync(string endpoint)
        {
            ThrowIfDisposed();

            // Send a GET request to the API
            var response = await _httpClient.GetAsync(endpoint);
            
            // Ensure the request was successful
            response.EnsureSuccessStatusCode();
            
            // Read and return the response content
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Posts data to a protected API endpoint
        /// </summary>
        /// <param name="endpoint">The API endpoint relative to the base URL</param>
        /// <param name="content">The content to send</param>
        /// <returns>The API response as a string</returns>
        public async Task<string> PostDataAsync(string endpoint, HttpContent content)
        {
            ThrowIfDisposed();

            // Send a POST request to the API
            var response = await _httpClient.PostAsync(endpoint, content);
            
            // Ensure the request was successful
            response.EnsureSuccessStatusCode();
            
            // Read and return the response content
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Throws an ObjectDisposedException if the service has been disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ApiService));
            }
        }

        /// <summary>
        /// Disposes the HTTP client when the service is disposed
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the resources used by the service
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _httpClient?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}