using DungeonMasterCompendium.Api.Options;
using Microsoft.Extensions.Options;
using System.Net;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Items
{
    public sealed class Open5eItemClient : IOpen5eItemClient
    {
        private readonly HttpClient _http;
        private readonly Open5eOptions _options;

        public Open5eItemClient(HttpClient http, IOptions<Open5eOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<Open5eItemListResponse> FetchItemList(string? name, int limit, CancellationToken cancellationToken)
        {
            string url = $"{_options.BaseUrl.TrimEnd('/')}/v1/magicitems/?limit={limit}";

            if (!string.IsNullOrWhiteSpace(name))
            {
                url += $"&search={Uri.EscapeDataString(name)}";
            }

            Open5eItemListResponse? response =
                await _http.GetFromJsonAsync<Open5eItemListResponse>(url, cancellationToken);

            return response ?? new Open5eItemListResponse();
        }

        public async Task<Open5eItemDetailItem?> FetchItemDetails(string externalId, CancellationToken cancellationToken)
        {
            string url = $"{_options.BaseUrl.TrimEnd('/')}/v1/magicitems/{externalId}/";

            HttpResponseMessage response = await _http.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            Open5eItemDetailItem? payload =
                await response.Content.ReadFromJsonAsync<Open5eItemDetailItem>(cancellationToken: cancellationToken);

            return payload;
        }
    }
}