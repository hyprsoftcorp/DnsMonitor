using Hyprsoft.Dns.Monitor.Providers;

namespace Hyprsoft.Dns.Monitor
{
    public class MonitorSettings : ProviderSettings
    {
        public int CheckIntervalMinutes { get; set; } = 10;
    }
}
