using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public abstract class ApiProvider(ILogger logger)
    {
        #region Properties

        protected ILogger Logger { get; } = logger;

        #endregion
    }
}
