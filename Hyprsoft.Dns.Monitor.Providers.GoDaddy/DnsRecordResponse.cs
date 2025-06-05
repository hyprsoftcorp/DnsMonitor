using System;

namespace Hyprsoft.Dns.Monitor.Providers
{
    internal class DnsRecordResponse
    {
        public string Data { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;

        public int Port { get; set; } = 1;

        public int Priority { get; set; }

        public string Protocol { get; set; } = String.Empty;

        public string Service { get; set; } = String.Empty;

        public int Ttl { get; set; }

        public string Type { get; set; } = String.Empty;

        public int Weight { get; set; } = 1;
    }
}
