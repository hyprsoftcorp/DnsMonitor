using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class IpifyPublicIpProvider : PublicIpProvider
    {
        #region Constructors

        internal IpifyPublicIpProvider(ILogger logger, string apiKey, string apiSecret) : base(logger, apiKey, apiSecret)
        {
            HttpClient.BaseAddress = new Uri("https://api.ipify.org/");
        }

        #endregion

        #region Properties

        public const string Key = nameof(IpifyPublicIpProvider);

        #endregion

        #region Methods

        public async override Task<string> GetPublicIPAddressAsync()
        {
            return await HttpClient.GetStringAsync("/");
        }

        #endregion
    }
}
