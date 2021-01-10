using Hyprsoft.Dns.Monitor.Providers;

namespace Hyprsoft.Dns.Monitor
{
    public class MonitorSettings
    {
        public bool IsFirstRun { get; set; } = true;

        public string[] Domains { get; set; } = new string[0];

        public ApiCredentials DnsProviderApiCredentials { get; set; } = new ApiCredentials { ProviderKey = HyprsoftDnsProvider.Key };

        public ApiCredentials PublicIpProviderApiCredentials { get; set; } = new ApiCredentials { ProviderKey = IpifyPublicIpProvider.Key };

        public int CheckIntervalMinutes { get; set; } = 10;
    }
}
