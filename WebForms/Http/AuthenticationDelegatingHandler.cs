using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebForms.Http
{
    /// <summary>
    /// HTTP message handler that adds an authentication token to outgoing requests
    /// </summary>
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly Func<Task<string>> _tokenProvider;
        private readonly string _scheme;

        /// <summary>
        /// Creates a new instance of the <see cref="AuthenticationDelegatingHandler"/>
        /// </summary>
        /// <param name="tokenProvider">A function that provides the authentication token</param>
        /// <param name="scheme">The authentication scheme (e.g., "Bearer")</param>
        /// <param name="innerHandler">Optional inner handler</param>
        public AuthenticationDelegatingHandler(
            Func<Task<string>> tokenProvider,
            string scheme = "Bearer",
            HttpMessageHandler innerHandler = null) 
            : base(innerHandler ?? new HttpClientHandler())
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation,
        /// adding the authentication token to the request
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation</param>
        /// <returns>The HTTP response message received from the server</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Get the token from the provider
            string token = await _tokenProvider().ConfigureAwait(false);

            // Add the token to the request Authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue(_scheme, token);

            // Call the inner handler
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}