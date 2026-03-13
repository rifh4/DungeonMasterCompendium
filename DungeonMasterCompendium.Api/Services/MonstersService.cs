using DungeonMasterCompendium.Api.Integrations.Open5e;

namespace DungeonMasterCompendium.Api.Services
{
    public sealed class MonstersService : IMonstersService
    {
        private readonly IOpen5eMonsterClient _open5eMonsterClient;

        public MonstersService(IOpen5eMonsterClient open5EMonsterClient)
        {
            _open5eMonsterClient = open5EMonsterClient;
        }

        public async Task<Open5eMonsterListResponse> GetMonsters(string? name, int limit, CancellationToken cancellationToken)
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

            int prefetchLimit = resolvedLimit * 5;
            if (prefetchLimit < 50)
            {
                prefetchLimit = 50;
            }
            else if (prefetchLimit > 100)
            {
                prefetchLimit = 100;
            }

            Open5eMonsterListResponse raw = await _open5eMonsterClient.FetchMonsterList(name, prefetchLimit, cancellationToken);

            if (string.IsNullOrWhiteSpace(name))
            {
                return new Open5eMonsterListResponse
                {
                    Count = raw.Count,
                    Next = null,
                    Previous = null,
                    Results = raw.Results.Take(resolvedLimit).ToList()
                };
            }

            string term = name.Trim();

            int Score(Open5eMonsterListItem item)
            {
                string slug = (item.Slug ?? string.Empty).Trim();
                string monsterName = (item.Name ?? string.Empty).Trim();

                if (slug.Equals(term, StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }

                if (monsterName.Equals(term, StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }

                if (slug.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    return 2;
                }

                if (monsterName.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    return 3;
                }

                return 4;
            }

            List<Open5eMonsterListItem> filtered = raw.Results
                .Select(item => new { Item = item, Rank = Score(item) })
                .Where(x => x.Rank < 4)
                .OrderBy(x => x.Rank)
                .ThenBy(x => x.Item.Name)
                .Select(x => x.Item)
                .ToList();

            List<Open5eMonsterListItem> limited = filtered.Take(resolvedLimit).ToList();

            return new Open5eMonsterListResponse
            {
                Count = filtered.Count,
                Next = null,
                Previous = null,
                Results = limited
            };
        }

    }
}
