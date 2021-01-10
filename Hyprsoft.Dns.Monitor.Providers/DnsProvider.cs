using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public interface IDnsProvider
    {
        Task CheckForChangesAsync(string[] domainNames);
    }

    public abstract class DnsProvider : SecureApiProvider, IDnsProvider
    {
        #region Fields

        private readonly IPublicIpProvider _publicIpProvider;
        private Dictionary<string, string> _dnsIPAddresses = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public DnsProvider(ILoggerFactory logger, IPublicIpProvider provider, ApiCredentials credentials) : base(logger, credentials) => _publicIpProvider = provider;

        #endregion

        #region Methods

        public async Task CheckForChangesAsync(string[] domainNames)
        {
            if (domainNames == null || domainNames.Length <= 0)
                return;

            var publicIpAddress = await _publicIpProvider.GetPublicIPAddressAsync();
            foreach (var domain in domainNames)
            {
                if (!_dnsIPAddresses.ContainsKey(domain))
                {
                    _dnsIPAddresses[domain] = await GetDnsIPAddressAsync(domain);
                    Logger.LogInformation($"Current DNS IP address for domain '{domain}' is '{_dnsIPAddresses[domain]}'.");
                }

                if (_dnsIPAddresses[domain].Equals(publicIpAddress))
                    Logger.LogInformation($"Current public IP address for domain '{domain}' is '{publicIpAddress}'.  No change detected.");
                else
                {
                    Logger.LogInformation($"New public IP address '{publicIpAddress}' detected.  Updating DNS record for domain '{domain}'.");
                    await SetDnsIPAddressAsync(domain, publicIpAddress);
                    _dnsIPAddresses[domain] = publicIpAddress;
                    Logger.LogInformation($"Domain '{domain}' updated successfully to IP address '{_dnsIPAddresses[domain]}'.");
                }
            }   // for each domain.
        }

        protected abstract Task<string> GetDnsIPAddressAsync(string domainName);

        protected abstract Task SetDnsIPAddressAsync(string domainName, string ip);

        #endregion
    }
}
