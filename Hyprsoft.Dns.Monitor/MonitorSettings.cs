using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Dns.Monitor.Providers.Common;
using System;

namespace Hyprsoft.Dns.Monitor
{
    public sealed class MonitorSettings : ProviderSettings
    {
        public string DnsProvider { get; set; } = HyprsoftDnsProvider.Key;
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(10);
    }
}
