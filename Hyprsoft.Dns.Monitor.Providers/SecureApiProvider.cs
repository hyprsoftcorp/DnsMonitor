using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class SecureApiProvider
    {
        #region Constructors

        public SecureApiProvider(ILoggerFactory logger, ApiCredentials credentials)
        {
            Logger = logger.CreateLogger(GetType());
            Credentials = credentials;
        }

        #endregion

        #region Properties

        protected ILogger Logger { get; }

        protected ApiCredentials Credentials { get; }

        #endregion
    }
}
