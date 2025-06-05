using Hyprsoft.Dns.Monitor.Providers.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor
{
    public sealed class Worker(ILogger<Worker> logger, MonitorSettings settings, IPublicIpProvider publicIpProvider, IDnsProvider dnsProvider) : BackgroundService
    {
        #region Fields

        private readonly ILogger<Worker> _logger = logger;
        private readonly MonitorSettings _settings = settings;
        private readonly IPublicIpProvider _publicIpProvider = publicIpProvider;
        private readonly IDnsProvider _dnsProvider = dnsProvider;

        #endregion

        #region Methods

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var product = (typeof(Worker).Assembly.GetCustomAttribute<AssemblyProductAttribute>())?.Product ?? "Hyprsoft Dns Monitor";
            var version = (typeof(Worker).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>())?.InformationalVersion ?? "0.0.0";
            var providersVersion = (typeof(PublicIpProvider).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>())?.InformationalVersion ?? "0.0.0";
            
            _logger.LogInformation("{Product} v{Version} | {Product} Providers v{ProvidersVersion}", product, version, product, providersVersion);

            if (_settings.Domains.Length <= 0)
                _logger.LogWarning($"The appsettings.json does NOT contain any domains.  At least one domain should be defined.");

            _logger.LogInformation("Checking for public IP changes every '{CheckIntervalMinutes}' minutes using IP provider '{PublicIpProviderName}' and DNS provider '{DnsProviderName}' for domains '{Domains}'.", 
                _settings.CheckInterval, _publicIpProvider.GetType().Name, _dnsProvider.GetType().Name, String.Join(", ", _settings.Domains));

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for public IP address changes.");
                    await _dnsProvider.CheckForChangesAsync(_settings.Domains, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to check for public IP address changes.");
                }
                _logger.LogInformation("Next check at '{CheckIntervalMinutes}'.", DateTime.Now.Add(_settings.CheckInterval));
                await Task.Delay(_settings.CheckInterval, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker exiting normally.");
            return base.StopAsync(cancellationToken);
        }

        #endregion
    }
}
