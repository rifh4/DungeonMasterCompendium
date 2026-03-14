using System.Text.Json;
using DungeonMasterCompendium.Api.Contracts.Spells;
using DungeonMasterCompendium.Api.Integrations.Open5e.Spells;
using DungeonMasterCompendium.Api.Services;
using Xunit;

namespace DungeonMasterCompendium.Tests
{
    public class SpellsServiceTests
    {
        [Fact]
        public async Task GetSpellDetails_MissThenHit_ExternalCalledOnce_AndCacheWritten()
        {
            FakeDistributedCache cache = new FakeDistributedCache();

            FakeOpen5eSpellClient client = new FakeOpen5eSpellClient
            {
                DetailResponse = new Open5eSpellDetailItem
                {
                    Slug = "fireball",
                    Name = "Fireball",
                    Level = "3",
                    School = "Evocation",
                    CastingTime = "1 action",
                    Range = "150 feet",
                    Duration = "Instantaneous",
                    Desc = "A bright streak flashes from your pointing finger to a point you choose.",
                    HigherLevel = "The damage increases by 1d6 for each slot level above 3rd.",
                    Components = "V, S, M",
                    Material = "A tiny ball of bat guano and sulfur",
                    Ritual = "no"
                }
            };

            SpellsService service = new SpellsService(client, cache);

            SpellDetailResponse? first = await service.GetSpellDetails(" Fireball ", CancellationToken.None);
            SpellDetailResponse? second = await service.GetSpellDetails("fireball", CancellationToken.None);

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.Equal("Fireball", first!.Name);
            Assert.Equal("Fireball", second!.Name);

            Assert.Equal(1, client.FetchSpellDetailsCallCount);
            Assert.Equal("fireball", client.LastDetailExternalId);

            Assert.Equal(1, cache.SetCallCount);
            Assert.Equal("dmcomp:spells:detail:fireball", cache.LastSetKey);
            Assert.NotNull(cache.LastSetOptions);
            Assert.Equal(TimeSpan.FromMinutes(10), cache.LastSetOptions!.AbsoluteExpirationRelativeToNow);

            string? cachedJson = cache.GetStoredString("dmcomp:spells:detail:fireball");
            Assert.NotNull(cachedJson);

            SpellDetailResponse? cachedPayload = JsonSerializer.Deserialize<SpellDetailResponse>(cachedJson!);
            Assert.NotNull(cachedPayload);
            Assert.Equal("Fireball", cachedPayload!.Name);
            Assert.Equal("Evocation", cachedPayload.School);
        }

        [Fact]
        public async Task GetSpells_MissThenHit_ExternalCalledOnce_AndCacheWritten()
        {
            FakeDistributedCache cache = new FakeDistributedCache();

            FakeOpen5eSpellClient client = new FakeOpen5eSpellClient
            {
                ListResponse = new Open5eSpellListResponse
                {
                    Count = 2,
                    Results = new List<Open5eSpellListItem>
                    {
                        new Open5eSpellListItem
                        {
                            Slug = "magic-missile",
                            Name = "Magic Missile",
                            Level = "1",
                            School = "Evocation",
                            CastingTime = "1 action",
                            Range = "120 feet"
                        },
                        new Open5eSpellListItem
                        {
                            Slug = "mage-armor",
                            Name = "Mage Armor",
                            Level = "1",
                            School = "Abjuration",
                            CastingTime = "1 action",
                            Range = "Touch"
                        }
                    }
                }
            };

            SpellsService service = new SpellsService(client, cache);

            SpellListResponse first = await service.GetSpells(" Magic ", 10, CancellationToken.None);
            SpellListResponse second = await service.GetSpells("magic", 10, CancellationToken.None);

            Assert.Equal(2, first.Count);
            Assert.Equal(2, second.Count);
            Assert.Equal(2, first.Results.Count);
            Assert.Equal(2, second.Results.Count);

            Assert.Equal(1, client.FetchSpellListCallCount);
            Assert.Equal("magic", client.LastListName);
            Assert.Equal(10, client.LastListLimit);

            Assert.Equal(1, cache.SetCallCount);
            Assert.Equal("dmcomp:spells:list:name:magic:limit:10", cache.LastSetKey);
            Assert.NotNull(cache.LastSetOptions);
            Assert.Equal(TimeSpan.FromMinutes(10), cache.LastSetOptions!.AbsoluteExpirationRelativeToNow);

            string? cachedJson = cache.GetStoredString("dmcomp:spells:list:name:magic:limit:10");
            Assert.NotNull(cachedJson);

            SpellListResponse? cachedPayload = JsonSerializer.Deserialize<SpellListResponse>(cachedJson!);
            Assert.NotNull(cachedPayload);
            Assert.Equal(2, cachedPayload!.Count);
            Assert.Equal(2, cachedPayload.Results.Count);
            Assert.Equal("Magic Missile", cachedPayload.Results[0].Name);
        }
    }
}