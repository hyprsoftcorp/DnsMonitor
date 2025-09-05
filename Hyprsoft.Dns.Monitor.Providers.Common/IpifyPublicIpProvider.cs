using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public sealed class IpifyPublicIpProvider : PublicIpProvider
    {
        public const string Key = nameof(IpifyPublicIpProvider);
        private readonly HttpClient _httpClient;

        public IpifyPublicIpProvider(ILogger<IpifyPublicIpProvider> logger, IHttpClientFactory httpClientFactory) : base(logger)
        {
            _httpClient = httpClientFactory.CreateClient(Key);
            _httpClient.BaseAddress = new Uri("https://api.ipify.org/");
        }

        public async override Task<string> GetPublicIPAddressAsync(CancellationToken cancellationToken) => await _httpClient.GetStringAsync("/");
    }
}
