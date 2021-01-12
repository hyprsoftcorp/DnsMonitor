using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftDnsProvider : DnsProvider
    {
        #region Constructors

        public HyprsoftDnsProvider(ILogger<HyprsoftDnsProvider> logger, IPublicIpProvider provider) : base(logger, provider) { }

        #endregion

        #region Properties

        public const string Key = nameof(HyprsoftDnsProvider);

        #endregion

        #region Methods

        protected override Task<string> GetDnsIpAddressAsync(string domainName)
        {
            Logger.LogWarning("This DNS provider is for testing purposes only and generates random IP addresses.");

            var rng = new Random((int)DateTime.Now.Ticks);
            return Task.FromResult($"{rng.Next(1, 256)}.{rng.Next(1, 256)}.{rng.Next(1, 256)}.{rng.Next(1, 256)}");
        }

        protected override Task SetDnsIpAddressAsync(string domainName, string ip)
        {
            Logger.LogWarning("This DNS provider is for testing purposes only and does NOT update any DNS records.");
            return Task.CompletedTask;
        }

        #endregion
    }
}
