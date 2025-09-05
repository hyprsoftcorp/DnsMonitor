using Microsoft.Extensions.Logging;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public abstract class ApiProvider(ILogger logger)
    {
        protected ILogger Logger { get; } = logger;
    }
}
