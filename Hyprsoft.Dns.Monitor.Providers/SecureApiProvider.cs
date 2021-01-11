using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class SecureApiProvider
    {
        #region Constructors

        public SecureApiProvider(ILogger logger, ApiCredentials credentials)
        {
            Logger = logger;
            Credentials = credentials;
        }

        #endregion

        #region Properties

        protected ILogger Logger { get; }

        protected ApiCredentials Credentials { get; }

        #endregion
    }
}
