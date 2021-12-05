using Hyprsoft.Dns.Monitor.Providers;
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
                .ConfigureAppConfiguration(builder => builder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, ConfigurationFilename), true))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDnsMonitor(settings => hostContext.Configuration.Bind(settings));
                    services.AddHostedService<Worker>();
                });
        }

        #endregion
    }
}
