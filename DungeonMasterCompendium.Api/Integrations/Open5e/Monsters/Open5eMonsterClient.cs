using DungeonMasterCompendium.Api.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Monsters
{
    public sealed class Open5eMonsterClient : IOpen5eMonsterClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<Open5eOptions> _options;

        public Open5eMonsterClient(HttpClient httpClient, IOptions<Open5eOptions> options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<Open5eMonsterListResponse> FetchMonsterList(string? name, int limit, CancellationToken cancellationToken)
        {
            int resolvedLimit;
            if (limit < 1)
            {
                resolvedLimit = 20;
            }
            else if (limit > 100)
            {
                resolvedLimit = 100;
            }
            else
            {
                resolvedLimit = limit;
            }

            string? trimmedName;
            if (string.IsNullOrWhiteSpace(name))
            {
                trimmedName = null;
            }
            else
            {
                trimmedName = name.Trim();
            }

            string baseUrl = _options.Value.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Open5e BaseUrl is missing from configuration.");
            }

            Uri baseUri = new Uri(baseUrl, UriKind.Absolute);
            Uri monstersUri = new Uri(baseUri, "v1/monsters/");

            Dictionary<string, string> query = new Dictionary<string, string>();
            query["limit"] = resolvedLimit.ToString();
            if (trimmedName != null)
            {
                query["search"] = trimmedName;
            }
            string requestUrl = QueryHelpers.AddQueryString(monstersUri.ToString(), query);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            Open5eMonsterListResponse? payload = await response.Content.ReadFromJsonAsync<Open5eMonsterListResponse>(cancellationToken: cancellationToken);
            if (payload == null)
            {
                throw new InvalidOperationException("The API returned an invalid response");
            }
            return payload;

        }

        public async Task<Open5eMonsterDetailItem?> FetchMonsterDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentException("Invalid API id");
            }
            string trimmedId = externalId.Trim();

            string baseUrl = _options.Value.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Open5e BaseUrl is missing from configuration.");
            }
            Uri baseUri = new Uri(baseUrl, UriKind.Absolute);
            Uri monsterDetailUri = new Uri(baseUri, "v1/monsters/" + trimmedId + "/");

            HttpResponseMessage response = await _httpClient.GetAsync(monsterDetailUri, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();

            Open5eMonsterDetailItem? payload = await response.Content.ReadFromJsonAsync<Open5eMonsterDetailItem>(cancellationToken: cancellationToken);
            if (payload == null)
            {
                throw new InvalidOperationException("The API returned an invalid response");
            }
            return payload;

        }
    }
}
