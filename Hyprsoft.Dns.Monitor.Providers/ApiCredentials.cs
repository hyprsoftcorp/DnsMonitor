namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class ApiCredentials
    {
        public ApiCredentials(string key) => ProviderKey = key;

        public string ProviderKey { get; set; }

        public string? ApiKey { get; set; }

        public string? ApiSecret { get; set; }
    }

    public class PublicIpProviderApiCredentials : ApiCredentials
    {
        public PublicIpProviderApiCredentials(string key) : base(key) { }
    }

    public class DnsProviderApiCredentials : ApiCredentials
    {
        public DnsProviderApiCredentials(string key) : base(key) { }
    }
}
