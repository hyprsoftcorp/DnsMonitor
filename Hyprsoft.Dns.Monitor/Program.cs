﻿using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
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
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppSettingsFilename, true)
                .AddUserSecrets<AppSettings>();

            var settings = new AppSettings();
            builder.Build().Bind(settings);

            var logManager = new SimpleLogManager();
            logManager.AddLogger(new SimpleFileLogger { MaxFileSizeBytes = settings.LogFileMaxFileSizeBytes });
            logManager.AddLogger(new SimpleConsoleLogger());

            try
            {
                if (settings.Domains == null || settings.Domains.Length <= 0)
                    throw new InvalidOperationException($"The '{AppSettingsFilename}' does not contain any domains.  At least one domain must be entered.");

                using (var cts = new CancellationTokenSource())
                {
                    Console.CancelKeyPress += (s, e) => cts.Cancel();
                    using (var ipProvider = PublicIpProvider.Create(logManager, settings.PublicIpProviderKey, settings.PublicIpProviderApiKey, settings.PublicIpProviderApiSecret))
                    {
                        using (var dnsProvider = DnsProvider.Create(logManager, ipProvider, settings.DnsProviderKey, settings.DnsProviderApiKey, settings.DnsProviderApiSecret))
                        {
                            await logManager.LogAsync<Program>(LogLevel.Info, $"Checking for public IP changes every '{settings.PollingDelayMinutes}' minutes using IP provider '{ipProvider.GetType().Name}' and DNS provider '{dnsProvider.GetType().Name}'.");
                            await RunAsync(dnsProvider, logManager, settings.Domains, TimeSpan.FromMinutes(settings.PollingDelayMinutes), cts.Token);
                        }   // using DNS provider.
                    }   // using public IP address providre.
                }   // using cancellation token source.
            }
            catch (Exception ex)
            {
                await logManager.LogAsync<Program>(ex, "Fatal application error.");
                Environment.Exit(1);
            }
            await logManager.LogAsync<Program>(LogLevel.Info, $"Process exiting.");
            Environment.Exit(0);
        }

        private static async Task RunAsync(DnsProvider provider, SimpleLogManager logManager, string[] domains, TimeSpan pollingDelay, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await logManager.LogAsync<Program>(LogLevel.Info, "Checking for public IP address changes.");
                        await provider.CheckForChangesAsync(domains);
                    }
                    catch (Exception ex)
                    {
                        await logManager.LogAsync<Program>(ex, "Unable to check for public IP address changes.");
                    }
                    await Task.Delay(pollingDelay, token);
                }   // while not cancelled.

            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                await logManager.LogAsync<Program>(ex, "Fatal application error.");
            }
        }

        #endregion
    }
}