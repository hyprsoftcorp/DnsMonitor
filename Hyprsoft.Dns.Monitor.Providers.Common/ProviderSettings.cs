using System;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public class ProviderSettings
    {
        public string[] Domains { get; set; } = [];

        public DnsProviderApiCredentials DnsProviderApiCredentials { get; set; }  = new DnsProviderApiCredentials();

        public PublicIpProviderApiCredentials PublicIpProviderApiCredentials { get; set; } = new PublicIpProviderApiCredentials();
    }
}
