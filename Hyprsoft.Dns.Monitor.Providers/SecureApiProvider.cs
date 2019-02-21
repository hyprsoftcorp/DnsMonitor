using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class SecureApiProvider
    {
        #region Constructors

        internal SecureApiProvider(ILogger logger, string apiKey, string apiSecret)
        {
            Logger = logger;
            ApiKey = apiKey;
            ApiSecret = apiSecret;
        }
        
        #endregion

        #region Properties

        protected ILogger Logger { get; }

        protected string ApiKey { get; }

        protected string ApiSecret { get; }

        #endregion
    }
}
