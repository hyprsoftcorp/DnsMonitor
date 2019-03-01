using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor
{
    class Program
    {
        #region Fields

        private const string AppSettingsFilename = "appsettings.json";

        #endregion

        #region Methods

        static async Task Main(string[] args)
        {
            /*  Setup user secrets file
            
            Edit .csproj and add:
            <PropertyGroup>
                 <UserSecretsId>A20304B9-DA98-407B-B05E-AAE4AF8C87F5</UserSecretsId>
            </PropertyGroup>

            Command prompt:
                1. cd to project folder.
                2. dotnet user-secrets set this that
                3. Open/edit %APPDATA%\microsoft\UserSecrets\A20304B9-DA98-407B-B05E-AAE4AF8C87F5\secrets.json
            */

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile(AppSettingsFilename, true);
#if DEBUG
            builder.AddUserSecrets<AppSettings>();
#endif
            var settings = new AppSettings();
            builder.Build().Bind(settings);

            var factory = new LoggerFactory();
            factory.AddConsole();
            factory.AddSimpleFileLogger();
            var logger = factory.CreateLogger<Program>();

            try
            {
                var title = (((AssemblyTitleAttribute)typeof(Program).Assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title);
                var version = (((AssemblyInformationalVersionAttribute)typeof(Program).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion);
                var providersVersion = (((AssemblyInformationalVersionAttribute)typeof(PublicIpProvider).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion); ;
                logger.LogInformation($"{title} v{version}, Providers v{providersVersion}");

                if (settings.Domains == null || settings.Domains.Length <= 0)
                    throw new InvalidOperationException($"The '{AppSettingsFilename}' does not contain any domains.  At least one domain must be entered.");

                using (var cts = new CancellationTokenSource())
                {
                    Console.CancelKeyPress += (s, e) =>
                    {
                        cts.Cancel();
                        e.Cancel = true;
                    };
                    using (var ipProvider = PublicIpProvider.Create(factory.CreateLogger<PublicIpProvider>(), settings.PublicIpProviderKey, settings.PublicIpProviderApiKey, settings.PublicIpProviderApiSecret))
                    {
                        using (var dnsProvider = DnsProvider.Create(factory.CreateLogger<DnsProvider>(), ipProvider, settings.DnsProviderKey, settings.DnsProviderApiKey, settings.DnsProviderApiSecret))
                        {
                            logger.LogInformation($"Checking for public IP changes every '{settings.PollingDelayMinutes}' minutes using IP provider '{ipProvider.GetType().Name}' and DNS provider '{dnsProvider.GetType().Name}'.");
                            await RunAsync(dnsProvider, logger, settings.Domains, TimeSpan.FromMinutes(settings.PollingDelayMinutes), cts.Token);
                        }   // using DNS provider.
                    }   // using public IP address providre.
                }   // using cancellation token source.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fatal application error.");
                Environment.Exit(1);
            }
            logger.LogInformation($"Process exiting.");
        }

        private static async Task RunAsync(DnsProvider provider, ILogger logger, string[] domains, TimeSpan pollingDelay, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        logger.LogInformation("Checking for public IP address changes.");
                        await provider.CheckForChangesAsync(domains);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Unable to check for public IP address changes.");
                    }
                    await Task.Delay(pollingDelay, token);
                }   // while not cancelled.
            }
            catch (TaskCanceledException)
            {
            }
        }

        #endregion
    }
}
