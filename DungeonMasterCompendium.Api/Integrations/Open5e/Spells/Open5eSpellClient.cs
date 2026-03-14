using System.Net;
using System.Net.Http.Json;
using DungeonMasterCompendium.Api.Options;
using Microsoft.Extensions.Options;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Spells
{
    public sealed class Open5eSpellClient : IOpen5eSpellClient
    {
        private readonly HttpClient _http;
        private readonly Open5eOptions _options;

        public Open5eSpellClient(HttpClient http, IOptions<Open5eOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<Open5eSpellListResponse> FetchSpellList(string? name, int limit, CancellationToken cancellationToken)
        {
            string url = $"{_options.BaseUrl.TrimEnd('/')}/v1/spells/?limit={limit}";

            if (!string.IsNullOrWhiteSpace(name))
            {
                url += $"&search={Uri.EscapeDataString(name)}";
            }

            Open5eSpellListResponse? response =
                await _http.GetFromJsonAsync<Open5eSpellListResponse>(url, cancellationToken);

            return response ?? new Open5eSpellListResponse();
        }

        public async Task<Open5eSpellDetailItem?> FetchSpellDetails(string externalId, CancellationToken cancellationToken)
        {
            string url = $"{_options.BaseUrl.TrimEnd('/')}/v1/spells/{externalId}/";

            HttpResponseMessage response = await _http.GetAsync(url, cancellationToken);

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
