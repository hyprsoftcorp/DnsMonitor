﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class GoDaddyDnsProvider : DnsProvider
    {
        #region GoDaddy Dns Record Class

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private class DnsRecord
        {
            public string data { get; set; } = String.Empty;

            public string name { get; set; } = String.Empty;

            public int port { get; set; } = 1;

            public int priority { get; set; }

            public string protocol { get; set; } = String.Empty;

            public string service { get; set; } = String.Empty;

            public int ttl { get; set; }

            public string type { get; set; } = String.Empty;

            public int weight { get; set; } = 1;
        }

        #endregion

        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Constructors

        public GoDaddyDnsProvider(ILogger<GoDaddyDnsProvider> logger, IPublicIpProvider provider, ProviderSettings settings, HttpClient httpClient) : base(logger, provider)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.godaddy.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"sso-key {settings.DnsProviderApiCredentials.ApiKey}:{settings.DnsProviderApiCredentials.ApiSecret}");
        }

        #endregion

        #region Properties

        public const string Key = nameof(GoDaddyDnsProvider);

        #endregion

        #region Methods

        protected override async Task<string> GetDnsIpAddressAsync(string domainName, CancellationToken cancellationToken = default) => (await GetDnsRecordAsync(domainName)).data;

        protected override async Task SetDnsIpAddressAsync(string domainName, string ip, CancellationToken cancellationToken = default)
        {
            // It's possible that changes have been made to the DNS record (i.e. TTL, etc.) since we last fetched it so let's fetch it again before we update.
            var record = await GetDnsRecordAsync(domainName);
            if (record.data.Equals(ip))
                return;

            record.data = ip;
            var response = await _httpClient.PutAsync(BuildApiEndpoint(domainName), new StringContent(JsonConvert.SerializeObject(new[] { record }), Encoding.UTF8, "application/json"), cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to set DNS record.  Reason: {response.ReasonPhrase}.  Details: {await response.Content.ReadAsStringAsync() ?? "none."}");
        }

        private async Task<DnsRecord> GetDnsRecordAsync(string domainName)
        {
            var response = await _httpClient.GetAsync(BuildApiEndpoint(domainName));
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to get DNS record.  Reason: {response.ReasonPhrase}.  Details: {await response.Content.ReadAsStringAsync() ?? "none."}");
            else
                return JsonConvert.DeserializeObject<List<DnsRecord>>(await response.Content.ReadAsStringAsync()).First();
        }

        private static string BuildApiEndpoint(string domainName) => $"v1/domains/{domainName.Substring(domainName.IndexOf(".") + 1)}/records/A/{domainName.Substring(0, domainName.IndexOf("."))}";

        #endregion
    }
}
