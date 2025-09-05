using Hyprsoft.Dns.Monitor.Providers.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class CloudflareDnsProvider : DnsProvider
    {
        public const string Key = nameof(CloudflareDnsProvider);
        private readonly HttpClient _httpClient;

        public CloudflareDnsProvider(ILogger<CloudflareDnsProvider> logger, IPublicIpProvider provider, ProviderSettings settings, IHttpClientFactory httpClientFactory) : base(logger, provider)
        {
            _httpClient = httpClientFactory.CreateClient(Key);
            _httpClient.BaseAddress = new Uri("https://api.cloudflare.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.DnsProviderApiCredentials.ApiSecret}");
        }

        protected override async Task<string> GetDnsIpAddressAsync(string domainName, CancellationToken cancellationToken) => (await GetDnsRecordAsync(domainName)).DnsRecord.Content;

        protected override async Task SetDnsIpAddressAsync(string domainName, string ip, CancellationToken cancellationToken)
        {
            // It's possible that changes have been made to the DNS record (i.e. TTL, etc.) since we last fetched it so let's fetch it again before we update.
            var (ZoneId, DnsRecord) = await GetDnsRecordAsync(domainName);
            if (string.Compare(DnsRecord.Content, ip, true) == 0)
                    return;

            DnsRecord.Content = ip; 
            var response = await _httpClient.PutAsync($"/client/v4/zones/{ZoneId}/dns_records/{DnsRecord.Id}", 
                new StringContent(JsonSerializer.Serialize(DnsRecord, JsonSerializerOptions), Encoding.UTF8, "application/json"), cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to set DNS record.  Reason: {response.ReasonPhrase}.  Details: {await response.Content.ReadAsStringAsync() ?? "none."}");
        }

        private async Task<(string ZoneId, DnsRecordResponse.ResultNested DnsRecord)> GetDnsRecordAsync(string domainName)
        {
            // Get our available zones
            var zoneId = string.Empty;
            var zoneResponse = await _httpClient.GetAsync("/client/v4/zones");
            if (!zoneResponse.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to get DNS zones.  Reason: {zoneResponse.ReasonPhrase}.  Details: {await zoneResponse.Content.ReadAsStringAsync() ?? "none."}");
            else
            {
                var (_, rootDomain) = GetDomainParts(domainName);
                var zoneRecord = JsonSerializer.Deserialize<ZoneRecordResponse>(await zoneResponse.Content.ReadAsStringAsync(), JsonSerializerOptions);
                zoneId = zoneRecord?.Result?.FirstOrDefault(x => x.Name.ToLower() == rootDomain)?.Id ?? throw new InvalidOperationException($"No DNS zone record found for domain '{rootDomain}'.");
            }

            // Get our A record for the specified zone id.
            var response = await _httpClient.GetAsync($"client/v4/zones/{zoneId}/dns_records");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to get DNS record.  Reason: {response.ReasonPhrase}.  Details: {await response.Content.ReadAsStringAsync() ?? "none."}");
            else
            {
                var dnsRecord = JsonSerializer.Deserialize<DnsRecordResponse>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);
                return (zoneId, dnsRecord?.Result?.FirstOrDefault(x => x.Name.ToLower() == domainName.ToLower() && x.Type == "A") ?? throw new InvalidOperationException($"No 'A' DNS record found for domain '{domainName}'."));
            }
        }
    }
}
