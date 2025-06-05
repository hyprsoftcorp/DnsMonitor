namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public abstract class ApiCredentials
    {
        public string? ApiKey { get; set; }

        public string? ApiSecret { get; set; }
    }

    public sealed class PublicIpProviderApiCredentials : ApiCredentials { }

    public sealed class DnsProviderApiCredentials : ApiCredentials { }
}
