using Hyprsoft.Web.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftPublicIpProvider : PublicIpProvider, IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly HyprsoftClient _client;

        #endregion

        #region Constructors

        public HyprsoftPublicIpProvider(ILogger<HyprsoftPublicIpProvider> logger, ProviderSettings settings, HttpClient httpClient) : base(logger, httpClient) => _client = new HyprsoftClient(settings.PublicIpProviderApiCredentials.ApiKey, settings.PublicIpProviderApiCredentials.ApiSecret);

        #endregion

        #region Properties

        public const string Key = nameof(HyprsoftPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync(CancellationToken cancellationToken = default) => await _client.GetPublicIPAddressAsync(cancellationToken);

        #endregion

        #region IDisposable

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

        #endregion
    }
}
