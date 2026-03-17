using System.Text.Json;
using DungeonMasterCompendium.Api.Contracts.Items;
using DungeonMasterCompendium.Api.Integrations.Open5e.Items;
using Microsoft.Extensions.Caching.Distributed;

namespace DungeonMasterCompendium.Api.Services
{
    public sealed class ItemsService : IItemsService
    {
        private readonly IOpen5eItemClient _open5eItemClient;
        private readonly IDistributedCache _cache;

        public ItemsService(IOpen5eItemClient open5eItemClient, IDistributedCache cache)
        {
            _open5eItemClient = open5eItemClient;
            _cache = cache;
        }

        public async Task<ItemListResponse> GetItems(string? name, int limit, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(name) && name.Trim().Length > 50)
            {
                throw new ArgumentException("Item name is too long.");
            }

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

            string normalizedName;
            if (string.IsNullOrWhiteSpace(name))
            {
                normalizedName = "all";
            }
            else
            {
                normalizedName = name.Trim().ToLowerInvariant();
            }

            string cacheKey = $"dmcomp:items:list:name:{normalizedName}:limit:{resolvedLimit}";
            string? cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedJson != null)
            {
                ItemListResponse? cachedResponse = JsonSerializer.Deserialize<ItemListResponse>(cachedJson);
                if (cachedResponse != null)
                {
                    return cachedResponse;
                }
            }

            string? upstreamName;
            if (normalizedName == "all")
            {
                upstreamName = null;
            }
            else
            {
                upstreamName = normalizedName;
            }

            Open5eItemListResponse raw =
                await _open5eItemClient.FetchItemList(upstreamName, resolvedLimit, cancellationToken);

            ItemListResponse response = new ItemListResponse
            {
                Count = raw.Count,
                Results = raw.Results
                    .Take(resolvedLimit)
                    .Select(item => new ItemListItemResponse
                    {
                        ExternalId = item.Slug ?? string.Empty,
                        Name = item.Name ?? string.Empty,
                        Type = item.Type ?? string.Empty,
                        Rarity = item.Rarity ?? string.Empty,
                        RequiresAttunement = item.RequiresAttunement ?? string.Empty
                    })
                    .ToList()
            };

            string responseJson = JsonSerializer.Serialize(response);

            // Cache list results for 10 minutes.
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            await _cache.SetStringAsync(cacheKey, responseJson, options, cancellationToken);

            return response;
        }

        public async Task<ItemDetailResponse?> GetItemDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentException("ExternalId is required.", nameof(externalId));
            }

            string normalizedExternalId = externalId.Trim().ToLowerInvariant();
            string cacheKey = $"dmcomp:items:detail:{normalizedExternalId}";
            string? cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedJson != null)
            {
                ItemDetailResponse? cachedResponse = JsonSerializer.Deserialize<ItemDetailResponse>(cachedJson);
                if (cachedResponse != null)
                {
                    return cachedResponse;
                }
            }

            Open5eItemDetailItem? detail =
                await _open5eItemClient.FetchItemDetails(normalizedExternalId, cancellationToken);

            if (detail == null)
            {
                return null;
            }

            ItemDetailResponse response = new ItemDetailResponse
            {
                ExternalId = detail.Slug ?? string.Empty,
                Name = detail.Name ?? string.Empty,
                Type = detail.Type ?? string.Empty,
                Rarity = detail.Rarity ?? string.Empty,
                RequiresAttunement = detail.RequiresAttunement ?? string.Empty,
                Description = detail.Desc ?? string.Empty
            };

            string responseJson = JsonSerializer.Serialize(response);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            await _cache.SetStringAsync(cacheKey, responseJson, options, cancellationToken);

            return response;
        }
    }
}