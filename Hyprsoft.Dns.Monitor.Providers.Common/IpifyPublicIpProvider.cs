using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public sealed class IpifyPublicIpProvider : PublicIpProvider
    {
        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Constructors

        public IpifyPublicIpProvider(ILogger<IpifyPublicIpProvider> logger, IHttpClientFactory httpClientFactory) : base(logger)
        {
            _httpClient = httpClientFactory.CreateClient(Key);
            _httpClient.BaseAddress = new Uri("https://api.ipify.org/");
        }

        #endregion

        #region Properties

        public const string Key = nameof(IpifyPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync(CancellationToken cancellationToken = default) => await _httpClient.GetStringAsync("/");

        #endregion
    }
}
