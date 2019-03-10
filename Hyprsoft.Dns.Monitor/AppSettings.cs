using Hyprsoft.Dns.Monitor.Providers;
using System;

namespace Hyprsoft.Dns.Monitor
{
    public class AppSettings
    {
        public bool FirstRun { get; set; } = true;

        public string[] Domains { get; set; }

        public string DnsProviderKey { get; set; } = HyprsoftDnsProvider.Key;

        public string DnsProviderApiKey { get; set; }

        public string DnsProviderApiSecret { get; set; }

        public string PublicIpProviderKey { get; set; } = IpifyPublicIpProvider.Key;

        public string PublicIpProviderApiKey { get; set; } 

        public string PublicIpProviderApiSecret { get; set; }

        public int LogFileMaxFileSizeBytes { get; set; } = 524288;  // 500k

        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(10);
    }
}
