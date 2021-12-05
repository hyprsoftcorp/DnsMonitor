using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public interface IDnsProvider
    {
        Task CheckForChangesAsync(string[] domainNames, CancellationToken cancellationToken);
    }

    public abstract class DnsProvider : ApiProvider, IDnsProvider
    {
        #region Fields

        private readonly IPublicIpProvider _publicIpProvider;
        private readonly Dictionary<string, string> _dnsIpAddresses = new();

        #endregion

        #region Constructors

        public DnsProvider(ILogger logger, IPublicIpProvider provider) : base(logger) => _publicIpProvider = provider;

        #endregion

        #region Methods

        public async Task CheckForChangesAsync(string[] domainNames, CancellationToken cancellationToken = default)
        {
            if (domainNames == null || domainNames.Length <= 0)
                return;

            var publicIpAddress = await _publicIpProvider.GetPublicIPAddressAsync(cancellationToken);
            foreach (var domain in domainNames)
            {
                if (!_dnsIpAddresses.ContainsKey(domain))
                {
                    _dnsIpAddresses[domain] = await GetDnsIpAddressAsync(domain, cancellationToken);
                    Logger.LogInformation($"Current DNS IP address for domain '{domain}' is '{_dnsIpAddresses[domain]}'.");
                }

                if (_dnsIpAddresses[domain].Equals(publicIpAddress))
                    Logger.LogInformation($"Current public IP address for domain '{domain}' is '{publicIpAddress}'.  No change detected.");
                else
                {
                    Logger.LogInformation($"New public IP address '{publicIpAddress}' detected.  Updating DNS record for domain '{domain}'.");
                    await SetDnsIpAddressAsync(domain, publicIpAddress, cancellationToken);
                    _dnsIpAddresses[domain] = publicIpAddress;
                    Logger.LogInformation($"Domain '{domain}' updated successfully to IP address '{_dnsIpAddresses[domain]}'.");
                }

                if (cancellationToken.IsCancellationRequested)
                    break;
            }   // for each domain.
        }

        protected abstract Task<string> GetDnsIpAddressAsync(string domainName, CancellationToken cancellationToken);

        protected abstract Task SetDnsIpAddressAsync(string domainName, string ip, CancellationToken cancellationToken);

        #endregion
    }
}
