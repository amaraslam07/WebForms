using Azure.Core;
using Azure.Identity;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebForms.Http
{
    /// <summary>
    /// HTTP message handler that adds Azure CLI authentication to outgoing requests
    /// </summary>
    public class AzureCliAuthenticationHandler : AuthenticationDelegatingHandler
    {
        private readonly string _scope;

        /// <summary>
        /// Creates a new instance of the <see cref="AzureCliAuthenticationHandler"/>
        /// </summary>
        /// <param name="scope">The Azure AD scope for the token</param>
        /// <param name="innerHandler">Optional inner handler</param>
        public AzureCliAuthenticationHandler(string scope, HttpMessageHandler innerHandler = null)
            : base(() => GetTokenAsync(scope), "Bearer", innerHandler)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <summary>
        /// Gets a token from Azure CLI
        /// </summary>
        /// <param name="scope">The scope for which to request the token</param>
        /// <returns>A token string</returns>
        private static async Task<string> GetTokenAsync(string scope)
        {
            try
            {
                // Create AzureCliCredential instance
                var credential = new AzureCliCredential();

                // Get the token for the requested scope
                var accessToken = await credential.GetTokenAsync(
                    new TokenRequestContext(new[] { scope }));

                return accessToken.Token;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Azure CLI authentication error: {ex.Message}");
                throw;
            }
        }
    }
}