using DungeonMasterCompendium.Api.Contracts.Monsters;
using DungeonMasterCompendium.Api.Integrations.Open5e;
using DungeonMasterCompendium.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace DungeonMasterCompendium.Tests
{
    public class MonstersServiceTests
    {
        [Fact]
        public async Task GetMonsterDetails_MissThenHit_ExternalCalledOnce_AndCacheWritten()
        {
            FakeDistributedCache cache = new FakeDistributedCache();

            FakeOpen5eMonsterClient client = new FakeOpen5eMonsterClient
            {
                DetailResponse = new Open5eMonsterDetailItem
                {
                    Slug = "bandit",
                    Name = "Bandit",
                    Size = "Medium",
                    Type = "Humanoid",
                    Alignment = "Any Non-Good Alignment",
                    ArmorClass = 12,
                    HitPoints = 11,
                    HitDice = "2d8+2",
                    Speed = new Dictionary<string, int>
                    {
                        ["walk"] = 30
                    },
                    Strength = 11,
                    Dexterity = 12,
                    Constitution = 12,
                    Intelligence = 10,
                    Wisdom = 10,
                    Charisma = 10,
                    ChallengeRating = "1/8"
                }
            };

            MonstersService service = new MonstersService(client, cache);

            MonsterDetailResponse? first = await service.GetMonsterDetails(" Bandit ", CancellationToken.None);
            MonsterDetailResponse? second = await service.GetMonsterDetails("bandit", CancellationToken.None);

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.Equal("Bandit", first!.Name);
            Assert.Equal("Bandit", second!.Name);

            Assert.Equal(1, client.FetchMonsterDetailsCallCount);
            Assert.Equal("bandit", client.LastDetailExternalId);

            Assert.Equal(1, cache.SetCallCount);
            Assert.Equal("dmcomp:monsters:detail:bandit", cache.LastSetKey);
            Assert.NotNull(cache.LastSetOptions);
            Assert.Equal(TimeSpan.FromMinutes(10), cache.LastSetOptions!.AbsoluteExpirationRelativeToNow);

            string? cachedJson = cache.GetStoredString("dmcomp:monsters:detail:bandit");
            Assert.NotNull(cachedJson);

            MonsterDetailResponse? cachedPayload = JsonSerializer.Deserialize<MonsterDetailResponse>(cachedJson!);
            Assert.NotNull(cachedPayload);
            Assert.Equal("Bandit", cachedPayload!.Name);
        }

        [Fact]
        public async Task GetMonsters_MissThenHit_ExternalCalledOnce_AndCacheWritten()
        {
            FakeDistributedCache cache = new FakeDistributedCache();

            FakeOpen5eMonsterClient client = new FakeOpen5eMonsterClient
            {
                ListResponse = new Open5eMonsterListResponse
                {
                    Count = 2,
                    Results = new List<Open5eMonsterListItem>
                    {
                        new Open5eMonsterListItem
                        {
                            Slug = "kobold",
                            Name = "Kobold",
                            Size = "Small",
                            Type = "Humanoid",
                            Alignment = "Lawful Evil",
                            ChallengeRating = "1/8"
                        },
                        new Open5eMonsterListItem
                        {
                            Slug = "kobold-dragonshield",
                            Name = "Kobold Dragonshield",
                            Size = "Small",
                            Type = "Humanoid",
                            Alignment = "Lawful Evil",
                            ChallengeRating = "1"
                        }
                    }
                }
            };

            MonstersService service = new MonstersService(client, cache);

            MonsterListResponse first = await service.GetMonsters(" Kobold ", 10, CancellationToken.None);
            MonsterListResponse second = await service.GetMonsters("kobold", 10, CancellationToken.None);

            Assert.Equal(2, first.Count);
            Assert.Equal(2, second.Count);
            Assert.Equal(2, first.Results.Count);
            Assert.Equal(2, second.Results.Count);

            Assert.Equal(1, client.FetchMonsterListCallCount);
            Assert.Equal("kobold", client.LastListName);
            Assert.Equal(50, client.LastListLimit);

            Assert.Equal(1, cache.SetCallCount);
            Assert.Equal("dmcomp:monsters:list:name:kobold:limit:10", cache.LastSetKey);
            Assert.NotNull(cache.LastSetOptions);
            Assert.Equal(TimeSpan.FromMinutes(10), cache.LastSetOptions!.AbsoluteExpirationRelativeToNow);

            string? cachedJson = cache.GetStoredString("dmcomp:monsters:list:name:kobold:limit:10");
            Assert.NotNull(cachedJson);

            MonsterListResponse? cachedPayload = JsonSerializer.Deserialize<MonsterListResponse>(cachedJson!);
            Assert.NotNull(cachedPayload);
            Assert.Equal(2, cachedPayload!.Count);
            Assert.Equal(2, cachedPayload.Results.Count);
            Assert.Equal("Kobold", cachedPayload.Results[0].Name);
        }
    }
}