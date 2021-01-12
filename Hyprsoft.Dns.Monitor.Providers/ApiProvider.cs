using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public abstract class ApiProvider
    {
        #region Constructors

        public ApiProvider(ILogger logger) => Logger = logger;

        #endregion

        #region Properties

        protected ILogger Logger { get; }

        #endregion
    }
}
