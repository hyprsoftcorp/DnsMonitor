using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDnsMonitor(this IServiceCollection services, Action<DnsMonitorSettings> configure)
        {
            var settings = new DnsMonitorSettings();
            configure?.Invoke(settings);

            services.AddSingleton(settings);
            services.AddSingleton(settings.PublicIpProviderApiCredentials);
            services.AddSingleton(settings.DnsProviderApiCredentials);

            services.AddHttpClient();

            if (settings.PublicIpProviderApiCredentials.ProviderKey == IpifyPublicIpProvider.Key)
                services.AddTransient<IPublicIpProvider, IpifyPublicIpProvider>();
            else if (settings.PublicIpProviderApiCredentials.ProviderKey == HyprsoftPublicIpProvider.Key)
                services.AddTransient<IPublicIpProvider, HyprsoftPublicIpProvider>();
            else
                throw new InvalidOperationException($"Public IP address provider key '{settings.PublicIpProviderApiCredentials.ProviderKey}' does not exist.  Valid values are '{IpifyPublicIpProvider.Key}' and '{HyprsoftPublicIpProvider.Key}'.");

            if (settings.DnsProviderApiCredentials.ProviderKey == GoDaddyDnsProvider.Key)
                services.AddTransient<IDnsProvider, GoDaddyDnsProvider>();
            else if (settings.DnsProviderApiCredentials.ProviderKey == HyprsoftDnsProvider.Key)
                services.AddTransient<IDnsProvider, HyprsoftDnsProvider>();
            else
                throw new InvalidOperationException($"DNS provider key '{settings.DnsProviderApiCredentials.ProviderKey}' does not exist.  Valid values are '{GoDaddyDnsProvider.Key}' and '{HyprsoftDnsProvider.Key}'.");

            return services;
        }
    }
}
