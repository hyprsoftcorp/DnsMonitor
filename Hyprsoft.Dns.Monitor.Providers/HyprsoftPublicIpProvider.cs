using Hyprsoft.Logging.Core;
using Hyprsoft.Web.Client;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftPublicIpProvider : PublicIpProvider
    {
        #region Fields

        private bool _isDisposed;
        private readonly HyprsoftClient _client;

        #endregion

        #region Constructors

        internal HyprsoftPublicIpProvider(SimpleLogManager logManager, string apiKey, string apiSecret) : base(logManager, apiKey, apiSecret)
        {
            _client = new HyprsoftClient(apiKey, apiSecret);
        }

        #endregion

        #region Properties

        public const string Key = nameof(HyprsoftPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync() => await _client.GetPublicIPAddressAsync();

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

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
