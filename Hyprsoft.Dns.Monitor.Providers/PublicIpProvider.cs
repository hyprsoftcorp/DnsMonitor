using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public interface IPublicIpProvider
    {
        Task<string> GetPublicIPAddressAsync();
    }

    public abstract class PublicIpProvider : SecureApiProvider, IPublicIpProvider
    {
        #region Constructors

        public PublicIpProvider(ILoggerFactory logger, ApiCredentials credentials, HttpClient httpClient) : base(logger, credentials) => HttpClient = httpClient;

        #endregion

        #region Properties

        protected HttpClient HttpClient { get; }

        #endregion

        #region Methods

        public abstract Task<string> GetPublicIPAddressAsync();

        #endregion
    }
}
