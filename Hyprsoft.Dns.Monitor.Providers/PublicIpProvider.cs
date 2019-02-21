﻿using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class PublicIpProvider : SecureApiProvider, IDisposable
    {
        #region Fields

        private bool _isDisposed;

        #endregion

        #region Constructors

        internal PublicIpProvider(ILogger logger, string apiKey, string apiSecret) : base(logger, apiKey, apiSecret)
        {
        }

        #endregion

        #region Properties

        protected HttpClient HttpClient { get; } = new HttpClient();

        #endregion

        #region Methods

        public static PublicIpProvider Create(ILogger logger, string providerKey, string apiKey, string apiSecret)
        {
            switch (providerKey)
            {
                case IpifyPublicIpProvider.Key:
                    return new IpifyPublicIpProvider(logger, apiKey, apiSecret);
                case HyprsoftPublicIpProvider.Key:
                    return new HyprsoftPublicIpProvider(logger, apiKey, apiSecret);
                default:
                    throw new InvalidOperationException($"Public IP address provider key '{providerKey}' does not exist.  Valid values are '{IpifyPublicIpProvider.Key}' and '{HyprsoftPublicIpProvider.Key}'.");
            }   // Public IP address provider key switch
        }

        public abstract Task<string> GetPublicIPAddressAsync();

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
            if (disposing)
                HttpClient?.Dispose();

            // Unmanaged resources.

            _isDisposed = true;
        }

        #endregion
    }
}
