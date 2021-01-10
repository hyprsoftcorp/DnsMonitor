using Hyprsoft.Web.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
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

        public HyprsoftPublicIpProvider(ILoggerFactory logger, ApiCredentials credentials, HttpClient httpClient) : base(logger, credentials, httpClient) => _client = new HyprsoftClient(credentials.ApiKey, credentials.ApiSecret);

        #endregion

        #region Properties

        public const string Key = nameof(HyprsoftPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync() => await _client.GetPublicIPAddressAsync();

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
