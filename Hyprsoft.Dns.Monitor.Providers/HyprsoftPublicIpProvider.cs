using Hyprsoft.Logging.Core;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftPublicIpProvider : PublicIpProvider
    {
        #region Fields

        private string _accessToken;

        #endregion

        #region Constructors

        internal HyprsoftPublicIpProvider(SimpleLogManager logManager, string apiKey, string apiSecret) : base(logManager, apiKey, apiSecret)
        {
            HttpClient.BaseAddress = new Uri("https://hyprsoftidentity.azurewebsites.net/");
        }

        #endregion

        #region Properties

        public const string Key = nameof(HyprsoftPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync()
        {
            if (String.IsNullOrWhiteSpace(_accessToken))
            {
                var disco = await HttpClient.GetDiscoveryDocumentAsync();
                if (disco.IsError)
                    throw new HttpRequestException(disco.Error);

                await LogManager.LogAsync<DnsProvider>(LogLevel.Info, $"Getting access token using '{disco.TokenEndpoint}'.");
                var tokenResponse = await HttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = ApiKey,
                    ClientSecret = ApiSecret,
                    Scope = "api"
                });
                if (tokenResponse.IsError)
                    throw new HttpRequestException(tokenResponse.Error);

                _accessToken = tokenResponse.AccessToken;
            }   // access token exists?

            HttpClient.SetBearerToken(_accessToken);
            var response = await HttpClient.GetAsync("api/myip");
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogManager.LogAsync<DnsProvider>(LogLevel.Warn, "An expired access token was detected.  Refreshing.");
                _accessToken = String.Empty;
                return await GetPublicIPAddressAsync();
            }
            else
                throw new HttpRequestException($"Unable to get public IP address.  Details: {response.ReasonPhrase} - {await response.Content.ReadAsStringAsync()}.");
        }

        #endregion
    }
}
