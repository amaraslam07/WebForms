using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms.DependencyInjection;
using WebForms.Http;
using WebForms.Services;

namespace WebForms
{
    public partial class _Default : Page
    {
        // Use dependency injection to get services
        private HttpClientFactory HttpClientFactory => this.Resolve<HttpClientFactory>();
        private ApiService ApiService => this.Resolve<ApiService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize page
        }

        protected void GetTokenButton_Click(object sender, EventArgs e)
        {
            // When working with async in ASP.NET WebForms, we need to register async task
            RegisterAsyncTask(new PageAsyncTask(GetTokenAsync));
        }

        // This method will be called asynchronously
        private async Task GetTokenAsync()
        {
            try
            {
                // Disable button during operation (needs to be done through script since we're async)
                ScriptManager.RegisterStartupScript(this, GetType(), "disableButton", 
                    "document.getElementById('" + GetTokenButton.ClientID + "').disabled = true;" +
                    "document.getElementById('" + GetTokenButton.ClientID + "').value = 'Getting token...';", true);

                // Get the token asynchronously
                string token = await GetTokenUsingAzureCliCredentialAsync();

                // Display the token (in a real app, you'd use this token to call your API)
                if (!string.IsNullOrEmpty(token))
                {
                    // Show only the first and last parts of the token for security
                    string maskedToken = MaskToken(token);
                    TokenResultLabel.Text = $"Token retrieved successfully: {maskedToken}";

                    // Example of using the HTTP client with token through DI
                    await DemonstrateHttpClientWithHandler();
                }
                else
                {
                    TokenResultLabel.Text = "Failed to retrieve token. Check if you're logged in with Azure CLI.";
                }
            }
            catch (Exception ex)
            {
                // Handle and display any errors
                TokenResultLabel.Text = $"Error: {ex.Message}";
            }
            finally
            {
                // Re-enable the button
                ScriptManager.RegisterStartupScript(this, GetType(), "enableButton", 
                    "document.getElementById('" + GetTokenButton.ClientID + "').disabled = false;" +
                    "document.getElementById('" + GetTokenButton.ClientID + "').value = 'Get Azure Token';", true);
            }
        }

        private string MaskToken(string token)
        {
            // Show only the first 10 and last 5 characters of the token for security
            if (token.Length > 20)
            {
                return $"{token.Substring(0, 10)}...{token.Substring(token.Length - 5)}";
            }
            return "***token too short to mask safely***";
        }

        protected async Task<string> GetTokenUsingAzureCliCredentialAsync()
        {
            try
            {
                // Create AzureCliCredential instance
                var credential = new AzureCliCredential();

                // Define the scope for your API
                var scope = "api://f9c13a4b-fe6e-4191-b63a-9f07864fc5b3/.default";

                // Get the token
                var accessToken = await credential.GetTokenAsync(
                    new TokenRequestContext(new[] { scope }));

                return accessToken.Token;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Azure CLI authentication error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Demonstrates how to use the HTTP client with the Azure authentication handler
        /// </summary>
        private async Task DemonstrateHttpClientWithHandler()
        {
            try
            {
                // Use injected ApiService if available
                if (ApiService != null)
                {
                    TokenResultLabel.Text += "<br/><br/>Using injected ApiService";
                    
                    // In a real application, you would call your API here
                    // var result = await ApiService.GetDataAsync("api/data");
                    // TokenResultLabel.Text += "<br/><br/>API response: " + result;
                }
                // Fallback to direct HttpClientFactory if ApiService is not available
                else if (HttpClientFactory != null)
                {
                    // The API scope - same as used for the token
                    string scope = "api://f9c13a4b-fe6e-4191-b63a-9f07864fc5b3/.default";
                    
                    // Create an HTTP client with Azure CLI authentication using injected factory
                    using (var client = HttpClientFactory.CreateAzureAuthenticatedClient(scope))
                    {
                        TokenResultLabel.Text += "<br/><br/>HTTP client created through injected HttpClientFactory.";
                    }
                }
                else
                {
                    // Fallback to static method for backward compatibility
                    string scope = "api://f9c13a4b-fe6e-4191-b63a-9f07864fc5b3/.default";
                    
                    using (var client = WebForms.Http.HttpClientFactory.CreateAzureAuthenticatedClientStatic(scope))
                    {
                        TokenResultLabel.Text += "<br/><br/>HTTP client created through static factory method (legacy).";
                    }
                }
            }
            catch (Exception ex)
            {
                TokenResultLabel.Text += $"<br/><br/>Error using HTTP client: {ex.Message}";
            }
        }
    }
}