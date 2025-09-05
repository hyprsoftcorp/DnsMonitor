using Hyprsoft.Dns.Monitor.Providers.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public sealed class GoDaddyDnsProvider : DnsProvider
    {
        private readonly HttpClient _httpClient;

        public GoDaddyDnsProvider(ILogger<GoDaddyDnsProvider> logger, IPublicIpProvider provider, ProviderSettings settings, IHttpClientFactory httpClientFactory) : base(logger, provider)
        {
            _httpClient = httpClientFactory.CreateClient(Key);
            _httpClient.BaseAddress = new Uri("https://api.godaddy.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"sso-key {settings.DnsProviderApiCredentials.ApiKey}:{settings.DnsProviderApiCredentials.ApiSecret}");
        }

        public const string Key = nameof(GoDaddyDnsProvider);

        protected override async Task<string> GetDnsIpAddressAsync(string domainName, CancellationToken cancellationToken) => (await GetDnsRecordAsync(domainName)).Data;

        protected override async Task SetDnsIpAddressAsync(string domainName, string ip, CancellationToken cancellationToken)
        {
            // It's possible that changes have been made to the DNS record (i.e. TTL, etc.) since we last fetched it so let's fetch it again before we update.
            var dnsRecord = await GetDnsRecordAsync(domainName);
            if (string.Compare(dnsRecord.Data, ip, true) == 0)
                return;

            dnsRecord.Data = ip;
            var response = await _httpClient.PutAsync(BuildApiEndpoint(domainName),
                new StringContent(JsonSerializer.Serialize(new[] { dnsRecord }, JsonSerializerOptions), Encoding.UTF8, "application/json"), cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to set DNS record.  Reason: {response.ReasonPhrase}.  Details: {await response.Content.ReadAsStringAsync() ?? "none."}");
        }

        private async Task<DnsRecordResponse> GetDnsRecordAsync(string domainName)
        {
            var response = await _httpClient.GetAsync(BuildApiEndpoint(domainName));
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to get DNS record.  Reason: {response.ReasonPhrase}.  Details: {await response.Content.ReadAsStringAsync() ?? "none."}");
            else
                return JsonSerializer.Deserialize<List<DnsRecordResponse>>(await response.Content.ReadAsStringAsync()).First();
        }

        private static string BuildApiEndpoint(string domainName)
        {
            var (subDomain, rootDomain) = GetDomainParts(domainName);
            
            return string.IsNullOrWhiteSpace(subDomain) ? $"v1/domains/{rootDomain}/records/A/@" : $"v1/domains/{rootDomain}/records/A/{subDomain}";
        }
    }
}
