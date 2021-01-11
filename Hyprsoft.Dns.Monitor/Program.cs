using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Hyprsoft.Dns.Monitor
{
    class Program
    {
        #region Properties

        public const string ConfigurationFilename = "config.json";

        #endregion

        #region Methods

        static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                .ConfigureLogging(builder => builder.AddSimpleFileLogger())
                .ConfigureAppConfiguration(builder => builder.AddJsonFile(ConfigurationFilename, true))
                .ConfigureServices((hostContext, services) =>
                {
                    var settings = ConfigureSettings(hostContext, services);
                    services.AddHttpClient();

                    services.AddTransient<IpifyPublicIpProvider>()
                        .AddTransient<HyprsoftPublicIpProvider>()
                        .AddTransient<IPublicIpProvider>(resolver => settings.PublicIpProviderApiCredentials.ProviderKey switch
                       {
                           IpifyPublicIpProvider.Key => resolver.GetRequiredService<IpifyPublicIpProvider>(),
                           HyprsoftPublicIpProvider.Key => resolver.GetRequiredService<HyprsoftPublicIpProvider>(),
                           _ => throw new InvalidOperationException($"Public IP address provider key '{settings.PublicIpProviderApiCredentials.ProviderKey}' does not exist.  Valid values are '{IpifyPublicIpProvider.Key}' and '{HyprsoftPublicIpProvider.Key}'.")
                       });

                    services.AddTransient<GoDaddyDnsProvider>()
                        .AddTransient<HyprsoftDnsProvider>()
                        .AddTransient<IDnsProvider>(resolver => settings.DnsProviderApiCredentials.ProviderKey switch
                        {
                            GoDaddyDnsProvider.Key => resolver.GetRequiredService<GoDaddyDnsProvider>(),
                            HyprsoftDnsProvider.Key => resolver.GetRequiredService<HyprsoftDnsProvider>(),
                            _ => throw new InvalidOperationException($"DNS provider key '{settings.DnsProviderApiCredentials.ProviderKey}' does not exist.  Valid values are '{GoDaddyDnsProvider.Key}' and '{HyprsoftDnsProvider.Key}'.")
                        });

                    services.AddHostedService<Worker>();
                });
        }

        private static MonitorSettings ConfigureSettings(HostBuilderContext hostContext, IServiceCollection services)
        {
            var settings = new MonitorSettings();
            hostContext.Configuration.Bind(settings);
            // Encrypt our sensitive settings if this is our first run.
            if (settings.IsFirstRun)
            {
                settings.DnsProviderApiCredentials.ApiSecret = DataProtector.EncryptString(settings.DnsProviderApiCredentials.ApiSecret);
                settings.PublicIpProviderApiCredentials.ApiSecret = DataProtector.EncryptString(settings.PublicIpProviderApiCredentials.ApiSecret);
                settings.IsFirstRun = false;
                var settingsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Program.ConfigurationFilename);
                File.WriteAllText(settingsFilename, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
            }   // First run?
            settings.DnsProviderApiCredentials.ApiSecret = DataProtector.DecryptString(settings.DnsProviderApiCredentials.ApiSecret);
            settings.PublicIpProviderApiCredentials.ApiSecret = DataProtector.DecryptString(settings.PublicIpProviderApiCredentials.ApiSecret);
            services.AddSingleton(settings);
            services.AddSingleton(settings.PublicIpProviderApiCredentials);
            services.AddSingleton(settings.DnsProviderApiCredentials);

            return settings;
        }

        #endregion
    }
}
