using Hyprsoft.Dns.Monitor.Providers.Common;
using Hyprsoft.Web.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftPublicIpProvider(ILogger<HyprsoftPublicIpProvider> logger, ProviderSettings settings) : PublicIpProvider(logger), IDisposable
    {
        private bool _isDisposed;
        private readonly HyprsoftClient _client = new(settings.PublicIpProviderApiCredentials.ApiKey, settings.PublicIpProviderApiCredentials.ApiSecret);

        public const string Key = nameof(HyprsoftPublicIpProvider);

        public async override Task<string> GetPublicIPAddressAsync(CancellationToken cancellationToken = default) => await _client.GetPublicIPAddressAsync(cancellationToken);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            // Managed resources.
            if (disposing)
                _client?.Dispose();

            // Unmanaged resources.

            _isDisposed = true;
        }
    }
}
