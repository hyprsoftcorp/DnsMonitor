namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class ApiCredentials
    {
        public string ProviderKey { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }
    }

    public class PublicIpProviderApiCredentials : ApiCredentials { }

    public class DnsProviderApiCredentials : ApiCredentials { }
}
