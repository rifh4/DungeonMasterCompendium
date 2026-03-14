using System.Net;
using DungeonMasterCompendium.Api.Options;
using Microsoft.Extensions.Options;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Spells
{
    public sealed class Open5eSpellClient : IOpen5eSpellClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public Open5eSpellClient(HttpClient http, IOptions<Open5eOptions> options)
        {
            _http = http;

            string? baseUrl = options.Value.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Open5e BaseUrl is not configured.");
            }

            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<Open5eSpellListResponse> FetchSpellList(string? name, int limit, CancellationToken cancellationToken)
        {
            string url = $"{_baseUrl}/v1/spells/?limit={limit}";

            if (!string.IsNullOrWhiteSpace(name))
            {
                string trimmedName = name.Trim();
                url += $"&search={Uri.EscapeDataString(trimmedName)}";
            }

            Open5eSpellListResponse? response =
                await _http.GetFromJsonAsync<Open5eSpellListResponse>(url, cancellationToken);

            return response ?? new Open5eSpellListResponse();
        }

        public async Task<Open5eSpellDetailItem?> FetchSpellDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentException("ExternalId is required.", nameof(externalId));
            }

            string url = $"{_baseUrl}/v1/spells/{externalId.Trim()}/";

            HttpResponseMessage response = await _http.GetAsync(url, cancellationToken);

            // A missing upstream spell is treated as "not found" in my service layer
            // instead of as an exceptional integration failure.
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            Open5eSpellDetailItem? payload =
                await response.Content.ReadFromJsonAsync<Open5eSpellDetailItem>(cancellationToken: cancellationToken);

            return payload;
        }
    }
}