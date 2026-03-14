using DungeonMasterCompendium.Api.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Monsters
{
    public sealed class Open5eMonsterClient : IOpen5eMonsterClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public Open5eMonsterClient(HttpClient http, IOptions<Open5eOptions> options)
        {
            _http = http;

            string? baseUrl = options.Value.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Open5e BaseUrl is not configured.");
            }

            _baseUrl = baseUrl.TrimEnd('/');
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

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                ["limit"] = resolvedLimit.ToString()
            };

            if (trimmedName != null)
            {
                query["search"] = trimmedName;
            }

            string requestUrl = QueryHelpers.AddQueryString($"{_baseUrl}/v1/monsters/", query);

            HttpResponseMessage response = await _http.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            Open5eMonsterListResponse? payload =
                await response.Content.ReadFromJsonAsync<Open5eMonsterListResponse>(cancellationToken: cancellationToken);

            if (payload == null)
            {
                throw new InvalidOperationException("The API returned an invalid response.");
            }

            return payload;
        }

        public async Task<Open5eMonsterDetailItem?> FetchMonsterDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentException("ExternalId is required.", nameof(externalId));
            }

            string requestUrl = $"{_baseUrl}/v1/monsters/{externalId.Trim()}/";

            HttpResponseMessage response = await _http.GetAsync(requestUrl, cancellationToken);

            // A missing upstream monster is treated as "not found" in my service layer
            // instead of as an exceptional integration failure.
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            Open5eMonsterDetailItem? payload =
                await response.Content.ReadFromJsonAsync<Open5eMonsterDetailItem>(cancellationToken: cancellationToken);

            if (payload == null)
            {
                throw new InvalidOperationException("The API returned an invalid response.");
            }

            return payload;
        }
    }
}