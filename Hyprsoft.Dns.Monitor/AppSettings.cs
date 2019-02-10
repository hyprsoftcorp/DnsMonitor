using Hyprsoft.Dns.Monitor.Providers;

namespace Hyprsoft.Dns.Monitor
{
    public class AppSettings
    {
        public string[] Domains { get; set; }

        public string DnsProviderKey { get; set; } = HyprsoftDnsProvider.Key;

        public string DnsProviderApiKey { get; set; }

        public string DnsProviderApiSecret { get; set; }

        public string PublicIpProviderKey { get; set; } = IpifyPublicIpProvider.Key;

        public string PublicIpProviderApiKey { get; set; } 

        public string PublicIpProviderApiSecret { get; set; }

        public int LogFileMaxFileSizeBytes { get; set; } = 524288;  // 500k

        public int PollingDelayMinutes { get; set; } = 10;
    }
}
