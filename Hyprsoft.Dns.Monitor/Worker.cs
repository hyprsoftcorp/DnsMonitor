using Hyprsoft.Dns.Monitor.Providers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor
{
    public class Worker : BackgroundService
    {
        #region Fields

        private readonly ILogger<Worker> _logger;
        private readonly MonitorSettings _settings;
        private readonly IPublicIpProvider _publicIpProvider;
        private readonly IDnsProvider _dnsProvider;

        #endregion

        #region Constructors

        public Worker(ILogger<Worker> logger, MonitorSettings settings, IPublicIpProvider publicIpProvider, IDnsProvider dnsProvider)
        {
            _logger = logger;
            _settings = settings;
            _publicIpProvider = publicIpProvider;
            _dnsProvider = dnsProvider;
        }

        #endregion

        #region Methods

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var product = (typeof(Worker).Assembly.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute)?.Product ?? "Hyprsoft Dns Monitor";
            var version = (typeof(Worker).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute)?.InformationalVersion ?? "0.0.0";
            var providersVersion = (typeof(PublicIpProvider).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute)?.InformationalVersion ?? "0.0.0";
            
            _logger.LogInformation($"{product} v{version} | {product} Providers v{providersVersion}");

            if (_settings.Domains.Length <= 0)
                _logger.LogWarning($"The appsettings.json does NOT contain any domains.  At least one domain should be defined.");

            _logger.LogInformation($"Checking for public IP changes every '{_settings.CheckIntervalMinutes}' minutes using IP provider '{_publicIpProvider.GetType().Name}' and DNS provider '{_dnsProvider.GetType().Name}' for domains '{String.Join(", ", _settings.Domains)}'.");

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
                _logger.LogInformation($"Next check at '{DateTime.Now.AddMinutes(_settings.CheckIntervalMinutes)}'.");
                await Task.Delay(TimeSpan.FromMinutes(_settings.CheckIntervalMinutes), stoppingToken);
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
