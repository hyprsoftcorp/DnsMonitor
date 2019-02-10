using Hyprsoft.Logging.Core;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class SecureApiProvider
    {
        #region Constructors

        internal SecureApiProvider(SimpleLogManager logManager, string apiKey, string apiSecret)
        {
            LogManager = logManager;
            ApiKey = apiKey;
            ApiSecret = apiSecret;
        }
        
        #endregion

        #region Properties

        protected SimpleLogManager LogManager { get; }

        protected string ApiKey { get; }

        protected string ApiSecret { get; }

        #endregion
    }
}
