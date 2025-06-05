using Hyprsoft.Dns.Monitor.Providers.Common;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCloudflareDnsMonitor(this IServiceCollection services, Action<ProviderSettings> configure)
        {
            var settings = new ProviderSettings();
            configure(settings);
            services.AddSingleton(settings);

            services.AddHttpClient(IpifyPublicIpProvider.Key);
            services.AddHttpClient(CloudflareDnsProvider.Key);

            services.AddTransient<IPublicIpProvider, IpifyPublicIpProvider>(); 
            services.AddTransient<IDnsProvider, CloudflareDnsProvider>();

            return services;
        }
    }
}
