using Hyprsoft.Logging.Core;
using System;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftDnsProvider : DnsProvider
    {
        #region Constructors

        internal HyprsoftDnsProvider(SimpleLogManager logManager, PublicIpProvider provider, string apiKey, string apiSecret) : base(logManager, provider, apiKey, apiSecret)
        {
        }

        #endregion

        #region Properties

        public const string Key = nameof(HyprsoftDnsProvider);

        #endregion

        #region Methods

        protected async override Task<string> GetDnsIPAddressAsync(string domainName)
        {
            await LogManager.LogAsync<HyprsoftDnsProvider>(LogLevel.Warn, "This DNS provider is for testing purposes only and generates random IP addresses.");

            var rng = new Random((int)DateTime.Now.Ticks);
            return $"{rng.Next(1, 256)}.{rng.Next(1, 256)}.{rng.Next(1, 256)}.{rng.Next(1, 256)}";
        }

        protected async override Task SetDnsIPAddressAsync(string domainName, string ip)
        {
            await LogManager.LogAsync<HyprsoftDnsProvider>(LogLevel.Warn, "This DNS provider is for testing purposes only and does NOT update any DNS records.");
        }

        #endregion
    }
}
