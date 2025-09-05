using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public interface IDnsProvider
    {
        Task CheckForChangesAsync(string[] domainNames, CancellationToken cancellationToken);
    }

    public abstract class DnsProvider(ILogger logger, IPublicIpProvider provider) : ApiProvider(logger), IDnsProvider
    {
        private readonly IPublicIpProvider _publicIpProvider = provider;
        private readonly Dictionary<string, string> _dnsIpAddresses = [];

        protected readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task CheckForChangesAsync(string[] domainNames, CancellationToken cancellationToken)
        {
            if (domainNames == null || domainNames.Length <= 0)
                return;

            var publicIpAddress = await _publicIpProvider.GetPublicIPAddressAsync(cancellationToken);
            foreach (var domain in domainNames)
            {
                if (!_dnsIpAddresses.ContainsKey(domain))
                {
                    _dnsIpAddresses[domain] = await GetDnsIpAddressAsync(domain, cancellationToken);
                    Logger.LogInformation("Current DNS IP address for domain '{Domain}' is '{IpAddress}'.", domain, _dnsIpAddresses[domain]);
                }

                if (string.Compare(_dnsIpAddresses[domain], publicIpAddress, true) == 0)
                    Logger.LogInformation("Current public IP address for domain '{Domain}' is '{IpAddress}'.  No change detected.", domain, publicIpAddress);
                else
                {
                    Logger.LogInformation("New public IP address '{IpAddress}' detected.  Updating DNS record for domain '{Domain}'.", publicIpAddress, domain);
                    await SetDnsIpAddressAsync(domain, publicIpAddress, cancellationToken);
                    _dnsIpAddresses[domain] = publicIpAddress;
                    Logger.LogInformation("Domain '{Domain}' updated successfully to IP address '{IpAddress}'.", domain, _dnsIpAddresses[domain]);
                }

                if (cancellationToken.IsCancellationRequested)
                    break;
            }   // for each domain.
        }

        protected static (string? SubDomain, string RootDomain) GetDomainParts(string domainName)
        {
            var isSubDomain = domainName.Count(x => x == '.') > 1;
            if (isSubDomain)
            {
                var secondToLast = domainName.LastIndexOf('.', domainName.LastIndexOf('.') - 1);
                var rootDomain = domainName.Substring(secondToLast + 1);
                var subDomain = domainName.Substring(0, secondToLast);

                return (subDomain, rootDomain);
            }
            else
                return (null, domainName);
        }

        protected abstract Task<string> GetDnsIpAddressAsync(string domainName, CancellationToken cancellationToken);

        protected abstract Task SetDnsIpAddressAsync(string domainName, string ip, CancellationToken cancellationToken);
    }
}
