using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public interface IDnsProvider
    {
        Task CheckForChangesAsync(string[] domainNames);
    }

    public abstract class DnsProvider : ApiProvider, IDnsProvider
    {
        #region Fields

        private readonly IPublicIpProvider _publicIpProvider;
        private Dictionary<string, string> _dnsIpAddresses = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public DnsProvider(ILogger logger, IPublicIpProvider provider) : base(logger) => _publicIpProvider = provider;

        #endregion

        #region Methods

        public async Task CheckForChangesAsync(string[] domainNames)
        {
            if (domainNames == null || domainNames.Length <= 0)
                return;

            var publicIpAddress = await _publicIpProvider.GetPublicIPAddressAsync();
            foreach (var domain in domainNames)
            {
                if (!_dnsIpAddresses.ContainsKey(domain))
                {
                    _dnsIpAddresses[domain] = await GetDnsIpAddressAsync(domain);
                    Logger.LogInformation($"Current DNS IP address for domain '{domain}' is '{_dnsIpAddresses[domain]}'.");
                }

                if (_dnsIpAddresses[domain].Equals(publicIpAddress))
                    Logger.LogInformation($"Current public IP address for domain '{domain}' is '{publicIpAddress}'.  No change detected.");
                else
                {
                    Logger.LogInformation($"New public IP address '{publicIpAddress}' detected.  Updating DNS record for domain '{domain}'.");
                    await SetDnsIpAddressAsync(domain, publicIpAddress);
                    _dnsIpAddresses[domain] = publicIpAddress;
                    Logger.LogInformation($"Domain '{domain}' updated successfully to IP address '{_dnsIpAddresses[domain]}'.");
                }
            }   // for each domain.
        }

        protected abstract Task<string> GetDnsIpAddressAsync(string domainName);

        protected abstract Task SetDnsIpAddressAsync(string domainName, string ip);

        #endregion
    }
}
