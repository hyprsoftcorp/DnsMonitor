using Hyprsoft.Dns.Monitor.Providers;
using System;

namespace Hyprsoft.Dns.Monitor
{
    public class MonitorSettings
    {
        public bool IsFirstRun { get; set; } = true;

        public string[] Domains { get; set; } = Array.Empty<string>();

        public DnsProviderApiCredentials DnsProviderApiCredentials { get; set; } = new DnsProviderApiCredentials { ProviderKey = HyprsoftDnsProvider.Key };

        public PublicIpProviderApiCredentials PublicIpProviderApiCredentials { get; set; } = new PublicIpProviderApiCredentials { ProviderKey = IpifyPublicIpProvider.Key };

        public int CheckIntervalMinutes { get; set; } = 10;
    }
}
