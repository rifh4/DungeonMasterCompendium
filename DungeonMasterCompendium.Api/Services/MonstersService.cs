using DungeonMasterCompendium.Api.Contracts.Monsters;
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

        public async Task<MonsterListResponse> GetMonsters(string? name, int limit, CancellationToken cancellationToken)
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

            MonsterListItemResponse Map(Open5eMonsterListItem item)
            {
                return new MonsterListItemResponse
                {
                    ExternalId = item.Slug ?? string.Empty,
                    Name = item.Name ?? string.Empty,
                    Size = item.Size ?? string.Empty,
                    Type = item.Type ?? string.Empty,
                    Alignment = item.Alignment ?? string.Empty,
                    ChallengeRating = item.ChallengeRating ?? string.Empty
                };
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return new MonsterListResponse
                {
                    Count = raw.Count,
                    Results = raw.Results.Take(resolvedLimit).Select(Map).ToList()
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

            return new MonsterListResponse
            {
                Count = filtered.Count,
                Results = limited.Select(Map).ToList()
            };
        }

        public async Task<MonsterDetailResponse?> GetMonsterDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentException("Empty API id");
            }

            Open5eMonsterDetailItem? detail = await _open5eMonsterClient.FetchMonsterDetails(externalId, cancellationToken);
            if (detail == null)
            {
                return null;
            }

            MonsterDetailResponse mapped = new MonsterDetailResponse
            {
                ExternalId = detail.Slug ?? string.Empty,
                Name = detail.Name ?? string.Empty,
                Size = detail.Size ?? string.Empty,
                Type = detail.Type ?? string.Empty,
                Alignment = detail.Alignment ?? string.Empty,
                ArmorClass = detail.ArmorClass,
                HitPoints = detail.HitPoints,
                HitDice = detail.HitDice ?? string.Empty,
                Speed = detail.Speed,
                Strength = detail.Strength,
                Dexterity = detail.Dexterity,
                Constitution = detail.Constitution,
                Intelligence = detail.Intelligence,
                Wisdom = detail.Wisdom,
                Charisma = detail.Charisma,
                ChallengeRating = detail.ChallengeRating ?? string.Empty
            };

            return mapped;
        }

    }
}
