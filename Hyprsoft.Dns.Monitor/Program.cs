using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Dns.Monitor.Providers.Common;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Hyprsoft.Dns.Monitor
{
    class Program
    {
        #region Methods

        static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                .ConfigureLogging(builder => builder.AddSimpleFileLogger())
                .ConfigureAppConfiguration(builder => builder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), true))
                .ConfigureServices((hostContext, services) =>
                {
                    var monitorSettings = new MonitorSettings();
                    hostContext.Configuration.GetSection(nameof(MonitorSettings)).Bind(monitorSettings);
                    services.AddSingleton(monitorSettings);

                    void ConfigureDnsMonitor(ProviderSettings settings)
                    {
                        settings.Domains = monitorSettings.Domains;
                        settings.DnsProviderApiCredentials = monitorSettings.DnsProviderApiCredentials;
                        settings.PublicIpProviderApiCredentials = monitorSettings.PublicIpProviderApiCredentials;
                    }

                    _ = monitorSettings.DnsProvider switch
                    {
                        CloudflareDnsProvider.Key => services.AddCloudflareDnsMonitor(ConfigureDnsMonitor),
                        GoDaddyDnsProvider.Key => services.AddGoDaddyDnsMonitor(ConfigureDnsMonitor),
                        _ => services.AddHyprsoftDnsMonitor(ConfigureDnsMonitor)
                    };

                    services.AddHostedService<Worker>();
                });
        }
        
        #endregion
    }
}