namespace Hyprsoft.Dns.Monitor.Providers
{
    public class ApiCredentials
    {
        public string ProviderKey { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public override string ToString() => $"ProviderKey: {ProviderKey} | ApiKey: {ApiKey} | ApiSecret: {ApiSecret}";
    }
}
