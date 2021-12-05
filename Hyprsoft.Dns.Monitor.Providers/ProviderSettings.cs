using System;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class ProviderSettings
    {
        public string[] Domains { get; set; } = Array.Empty<string>();

        public DnsProviderApiCredentials DnsProviderApiCredentials { get; set; } = new DnsProviderApiCredentials(HyprsoftDnsProvider.Key);

        public PublicIpProviderApiCredentials PublicIpProviderApiCredentials { get; set; } = new PublicIpProviderApiCredentials(IpifyPublicIpProvider.Key);
    }
}
