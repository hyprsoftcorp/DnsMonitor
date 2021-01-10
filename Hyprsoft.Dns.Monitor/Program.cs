using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
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

                    services.AddHttpClient();
                    services.AddTransient<IPublicIpProvider>(resolver => CreatePublicIpProvider(resolver, settings.PublicIpProviderApiCredentials));
                    services.AddTransient<IDnsProvider>(resolver => CreateDnsProvider(resolver, settings.DnsProviderApiCredentials));
                    services.AddHostedService<Worker>();
                });
        }

        private static IPublicIpProvider CreatePublicIpProvider(IServiceProvider resolver, ApiCredentials credentials) => credentials.ProviderKey switch
        {
            IpifyPublicIpProvider.Key => new IpifyPublicIpProvider(resolver.GetRequiredService<ILoggerFactory>(), credentials, resolver.GetRequiredService<HttpClient>()),
            HyprsoftPublicIpProvider.Key => new HyprsoftPublicIpProvider(resolver.GetRequiredService<ILoggerFactory>(), credentials, resolver.GetRequiredService<HttpClient>()),
            _ => throw new InvalidOperationException($"Public IP address provider key '{credentials.ProviderKey}' does not exist.  Valid values are '{IpifyPublicIpProvider.Key}' and '{HyprsoftPublicIpProvider.Key}'.")
        };

        private static IDnsProvider CreateDnsProvider(IServiceProvider resolver, ApiCredentials credentials) => credentials.ProviderKey switch
        {
            GoDaddyDnsProvider.Key => new GoDaddyDnsProvider(resolver.GetRequiredService<ILoggerFactory>(), resolver.GetRequiredService<IPublicIpProvider>(), credentials, resolver.GetRequiredService<HttpClient>()),
            HyprsoftDnsProvider.Key => new HyprsoftDnsProvider(resolver.GetRequiredService<ILoggerFactory>(), resolver.GetRequiredService<IPublicIpProvider>(), credentials),
            _ => throw new InvalidOperationException($"DNS provider key '{credentials.ProviderKey}' does not exist.  Valid values are '{GoDaddyDnsProvider.Key}' and '{HyprsoftDnsProvider.Key}'.")
        };

        #endregion
    }
}
