using Hyprsoft.Dns.Monitor.Providers;
using Hyprsoft.Logging.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor
{
    class Program
    {
        #region Fields

        private const string AppSettingsFilename = "appsettings.json";
        private const string DataProtectionApplicationName = "Hyprsoft.Dns.Monitor";

        #endregion

        #region Methods

        static async Task Main(string[] args)
        {
            var factory = new LoggerFactory();
#pragma warning disable CS0618 // Type or member is obsolete
            factory.AddConsole();
#pragma warning restore CS0618 // Type or member is obsolete
            factory.AddSimpleFileLogger();
            var logger = factory.CreateLogger<Program>();

            try
            {
                var product = (((AssemblyProductAttribute)typeof(Program).Assembly.GetCustomAttribute(typeof(AssemblyProductAttribute))).Product);
                var version = (((AssemblyInformationalVersionAttribute)typeof(Program).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion);
                var providersVersion = (((AssemblyInformationalVersionAttribute)typeof(PublicIpProvider).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion); ;
                logger.LogInformation($"{product} v{version}, Providers v{providersVersion}");

                var settings = new AppSettings();
                var settingsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), AppSettingsFilename).ToLower();
                if (File.Exists(settingsFilename))
                {
                    logger.LogInformation($"Loading app settings from '{settingsFilename}'.");
                    settings = JsonConvert.DeserializeObject<AppSettings>(await File.ReadAllTextAsync(settingsFilename));
                }
                else
                    await File.WriteAllTextAsync(settingsFilename, JsonConvert.SerializeObject(settings, Formatting.Indented));

                if (settings.Domains == null || settings.Domains.Length <= 0)
                    throw new InvalidOperationException($"The '{AppSettingsFilename}' does not contain any domains.  At least one domain must be entered.");

                // Encrypt our sensitive settings if this is our first run.
                if (settings.FirstRun)
                {
                    logger.LogWarning("First run detected.  Encrypting sensitive settings.");
                    if (!String.IsNullOrEmpty(settings.DnsProviderApiSecret))
                        settings.DnsProviderApiSecret = EncryptString(settings.DnsProviderApiSecret);
                    if (!String.IsNullOrEmpty(settings.PublicIpProviderApiSecret))
                        settings.PublicIpProviderApiSecret = EncryptString(settings.PublicIpProviderApiSecret);
                    settings.FirstRun = false;
                    logger.LogInformation($"Saving app settings to '{settingsFilename}'.");
                    await File.WriteAllTextAsync(settingsFilename, JsonConvert.SerializeObject(settings, Formatting.Indented));
                }   // First run?
                if (!String.IsNullOrEmpty(settings.DnsProviderApiSecret))
                    settings.DnsProviderApiSecret = DecryptString(settings.DnsProviderApiSecret);
                if (!String.IsNullOrEmpty(settings.PublicIpProviderApiSecret))
                    settings.PublicIpProviderApiSecret = DecryptString(settings.PublicIpProviderApiSecret);

                using (var cts = new CancellationTokenSource())
                {
                    Console.WriteLine("\nPress Ctrl+C to exit.\n");
                    Console.CancelKeyPress += (s, e) =>
                    {
                        cts.Cancel();
                        e.Cancel = true;
                    };
                    using (var ipProvider = PublicIpProvider.Create(factory.CreateLogger<PublicIpProvider>(), settings.PublicIpProviderKey, settings.PublicIpProviderApiKey, settings.PublicIpProviderApiSecret))
                    {
                        using (var dnsProvider = DnsProvider.Create(factory.CreateLogger<DnsProvider>(), ipProvider, settings.DnsProviderKey, settings.DnsProviderApiKey, settings.DnsProviderApiSecret))
                        {
                            logger.LogInformation($"Checking for public IP changes every '{settings.CheckInterval.TotalMinutes}' minutes using IP provider '{ipProvider.GetType().Name}' and DNS provider '{dnsProvider.GetType().Name}'.");
                            try
                            {
                                while (!cts.Token.IsCancellationRequested)
                                {
                                    try
                                    {
                                        logger.LogInformation("Checking for public IP address changes.");
                                        await dnsProvider.CheckForChangesAsync(settings.Domains);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, "Unable to check for public IP address changes.");
                                    }
                                        logger.LogInformation($"Next check at '{DateTime.Now.Add(settings.CheckInterval)}'.");
                                    await Task.Delay(settings.CheckInterval, cts.Token);
                                }   // while not cancelled.
                            }
                            catch (TaskCanceledException)
                            {
                            }
                        }   // using DNS provider.
                    }   // using public IP address providre.
                }   // using cancellation token source.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Fatal application error.  Details: {ex.Message}");
                Environment.Exit(1);
            }
            logger.LogInformation($"Process exiting normally.");
        }

        internal static string EncryptString(string plainText)
        {
            var dp = DataProtectionProvider.Create(DataProtectionApplicationName);
            var protector = dp.CreateProtector(DataProtectionApplicationName);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(protector.Protect(plainText)));
        }

        internal static string DecryptString(string secret)
        {
            var dp = DataProtectionProvider.Create(DataProtectionApplicationName);
            var protector = dp.CreateProtector(DataProtectionApplicationName);
            return protector.Unprotect(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(secret)));
        }

        #endregion
    }
}
