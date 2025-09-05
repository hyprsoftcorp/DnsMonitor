using Hyprsoft.Dns.Monitor.Providers.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class HyprsoftDnsProvider(ILogger<HyprsoftDnsProvider> logger, IPublicIpProvider provider) : DnsProvider(logger, provider)
    {
        public const string Key = nameof(HyprsoftDnsProvider);

        protected override Task<string> GetDnsIpAddressAsync(string domainName, CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("This DNS provider is for testing purposes only and generates random IP addresses.");

            var rng = new Random((int)DateTime.Now.Ticks);
            return Task.FromResult($"{rng.Next(1, 256)}.{rng.Next(1, 256)}.{rng.Next(1, 256)}.{rng.Next(1, 256)}");
        }

        protected override Task SetDnsIpAddressAsync(string domainName, string ip, CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("This DNS provider is for testing purposes only and does NOT update any DNS records.");
            return Task.CompletedTask;
        }
    }
}
