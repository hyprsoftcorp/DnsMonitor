using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class IpifyPublicIpProvider : PublicIpProvider
    {
        #region Constructors

        public IpifyPublicIpProvider(ILogger<IpifyPublicIpProvider> logger, PublicIpProviderApiCredentials credentials, HttpClient httpClient) : base(logger, credentials, httpClient) => HttpClient.BaseAddress = new Uri("https://api.ipify.org/");

        #endregion

        #region Properties

        public const string Key = nameof(IpifyPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync() => await HttpClient.GetStringAsync("/");

        #endregion
    }
}
