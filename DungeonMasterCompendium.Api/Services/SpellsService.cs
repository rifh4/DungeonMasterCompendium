using System.Text.Json;
using DungeonMasterCompendium.Api.Contracts.Spells;
using DungeonMasterCompendium.Api.Integrations.Open5e.Spells;
using Microsoft.Extensions.Caching.Distributed;

namespace DungeonMasterCompendium.Api.Services
{
    public sealed class SpellsService : ISpellsService
    {
        private readonly IOpen5eSpellClient _open5eSpellClient;
        private readonly IDistributedCache _cache;

        public SpellsService(IOpen5eSpellClient open5eSpellClient, IDistributedCache cache)
        {
            _open5eSpellClient = open5eSpellClient;
            _cache = cache;
        }

        public async Task<SpellListResponse> GetSpells(string? name, int limit, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(name) && name.Trim().Length > 50)
            {
                throw new ArgumentException("Spell name is too long.");
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

            string cacheKey = $"dmcomp:spells:list:name:{normalizedName}:limit:{resolvedLimit}";
            string? cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedJson != null)
            {
                SpellListResponse? cachedResponse = JsonSerializer.Deserialize<SpellListResponse>(cachedJson);
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

            Open5eSpellListResponse raw =
                await _open5eSpellClient.FetchSpellList(upstreamName, resolvedLimit, cancellationToken);

            SpellListResponse response = new SpellListResponse
            {
                Count = raw.Count,
                Results = raw.Results
                    .Take(resolvedLimit)
                    .Select(item => new SpellListItemResponse
                    {
                        ExternalId = item.Slug ?? string.Empty,
                        Name = item.Name ?? string.Empty,
                        Level = item.Level ?? string.Empty,
                        School = item.School ?? string.Empty,
                        CastingTime = item.CastingTime ?? string.Empty,
                        Range = item.Range ?? string.Empty
                    })
                    .ToList()
            };

            string responseJson = JsonSerializer.Serialize(response);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            await _cache.SetStringAsync(cacheKey, responseJson, options, cancellationToken);

            return response;
        }

        public async Task<SpellDetailResponse?> GetSpellDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentException("Empty API id");
            }

            string normalizedExternalId = externalId.Trim().ToLowerInvariant();
            string cacheKey = $"dmcomp:spells:detail:{normalizedExternalId}";
            string? cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedJson != null)
            {
                SpellDetailResponse? cachedResponse = JsonSerializer.Deserialize<SpellDetailResponse>(cachedJson);
                if (cachedResponse != null)
                {
                    return cachedResponse;
                }
            }

            Open5eSpellDetailItem? detail =
                await _open5eSpellClient.FetchSpellDetails(normalizedExternalId, cancellationToken);

            if (detail == null)
            {
                return null;
            }

            SpellDetailResponse response = new SpellDetailResponse
            {
                ExternalId = detail.Slug ?? string.Empty,
                Name = detail.Name ?? string.Empty,
                Level = detail.Level ?? string.Empty,
                School = detail.School ?? string.Empty,
                CastingTime = detail.CastingTime ?? string.Empty,
                Range = detail.Range ?? string.Empty,
                Duration = detail.Duration ?? string.Empty,
                Description = detail.Desc ?? string.Empty,
                HigherLevel = detail.HigherLevel ?? string.Empty,
                Components = detail.Components ?? string.Empty,
                Material = detail.Material ?? string.Empty,
                Ritual = detail.Ritual ?? string.Empty
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
