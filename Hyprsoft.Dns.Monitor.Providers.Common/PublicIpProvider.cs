using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public interface IPublicIpProvider
    {
        Task<string> GetPublicIPAddressAsync(CancellationToken cancellationToken);
    }

    public abstract class PublicIpProvider(ILogger logger) : ApiProvider(logger), IPublicIpProvider
    {
        #region Methods

        public abstract Task<string> GetPublicIPAddressAsync(CancellationToken cancellationToken);

        #endregion
    }
}
