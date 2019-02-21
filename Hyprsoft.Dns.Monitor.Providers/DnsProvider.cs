using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class DnsProvider : SecureApiProvider, IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly PublicIpProvider _publicIPAddressProvider;
        private Dictionary<string, string> _dnsIPAddresses = new Dictionary<string, string>();

        #endregion

        #region Constructors

        internal DnsProvider(ILogger logger, PublicIpProvider provider, string apiKey, string apiSecret) : base(logger, apiKey, apiSecret)
        {
            _publicIPAddressProvider = provider;
        }

        #endregion

        #region Methods

        public static DnsProvider Create(ILogger logger, PublicIpProvider provider, string providerKey, string apiKey, string apiSecret)
        {
            switch (providerKey)
            {
                case GoDaddyDnsProvider.Key:
                    return new GoDaddyDnsProvider(logger, provider, apiKey, apiSecret);
                case HyprsoftDnsProvider.Key:
                    return new HyprsoftDnsProvider(logger, provider, apiKey, apiSecret);
                default:
                    throw new InvalidOperationException($"DNS provider key '{providerKey}' does not exist.  Valid values are '{GoDaddyDnsProvider.Key}' and '{HyprsoftDnsProvider.Key}'.");
            }   // DNS provider key switch
        }

        public async Task CheckForChangesAsync(string[] domainNames)
        {
            var publicIPAddress = await _publicIPAddressProvider.GetPublicIPAddressAsync();
            foreach (var domain in domainNames)
            {
                if (!_dnsIPAddresses.ContainsKey(domain))
                {
                    _dnsIPAddresses[domain] = await GetDnsIPAddressAsync(domain);
                    Logger.LogInformation($"Current DNS IP address for domain '{domain}' is '{_dnsIPAddresses[domain]}'.");
                }

                if (_dnsIPAddresses[domain].Equals(publicIPAddress))
                    Logger.LogInformation($"Current public IP address for domain '{domain}' is '{publicIPAddress}'.  No change detected.");
                else
                {
                    Logger.LogInformation($"New public IP address '{publicIPAddress}' detected.  Updating DNS record for domain '{domain}'.");
                    await SetDnsIPAddressAsync(domain, publicIPAddress);
                    _dnsIPAddresses[domain] = publicIPAddress;
                    Logger.LogInformation($"Domain '{domain}' updated successfully to IP address '{_dnsIPAddresses[domain]}'.");
                }
            }   // for each domain.
        }

        protected abstract Task<string> GetDnsIPAddressAsync(string domainName);

        protected abstract Task SetDnsIPAddressAsync(string domainName, string ip);

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            // Managed resources.
            if (disposing) { }

            // Unmanaged resources.

            _isDisposed = true;
        }

        #endregion
    }
}
