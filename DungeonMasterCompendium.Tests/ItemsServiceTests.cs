using System.Text.Json;
using DungeonMasterCompendium.Api.Contracts.Items;
using DungeonMasterCompendium.Api.Integrations.Open5e.Items;
using DungeonMasterCompendium.Api.Services;
using Xunit;

namespace DungeonMasterCompendium.Tests
{
    public class ItemsServiceTests
    {
        [Fact]
        public async Task GetItemDetails_MissThenHit_ExternalCalledOnce_AndCacheWritten()
        {
            FakeDistributedCache cache = new FakeDistributedCache();

            FakeOpen5eItemClient client = new FakeOpen5eItemClient
            {
                DetailResponse = new Open5eItemDetailItem
                {
                    Slug = "bag-of-holding",
                    Name = "Bag of Holding",
                    Type = "Wondrous Item",
                    Rarity = "uncommon",
                    RequiresAttunement = "no",
                    Desc = "This bag has an interior space considerably larger than its outside dimensions."
                }
            };

            ItemsService service = new ItemsService(client, cache);

            ItemDetailResponse? first = await service.GetItemDetails(" Bag-of-Holding ", CancellationToken.None);
            ItemDetailResponse? second = await service.GetItemDetails("bag-of-holding", CancellationToken.None);

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.Equal("Bag of Holding", first!.Name);
            Assert.Equal("Bag of Holding", second!.Name);

            Assert.Equal(1, client.FetchItemDetailsCallCount);
            Assert.Equal("bag-of-holding", client.LastDetailExternalId);

            Assert.Equal(1, cache.SetCallCount);
            Assert.Equal("dmcomp:items:detail:bag-of-holding", cache.LastSetKey);
            Assert.NotNull(cache.LastSetOptions);
            Assert.Equal(TimeSpan.FromMinutes(10), cache.LastSetOptions!.AbsoluteExpirationRelativeToNow);

            string? cachedJson = cache.GetStoredString("dmcomp:items:detail:bag-of-holding");
            Assert.NotNull(cachedJson);

            ItemDetailResponse? cachedPayload = JsonSerializer.Deserialize<ItemDetailResponse>(cachedJson!);
            Assert.NotNull(cachedPayload);
            Assert.Equal("Bag of Holding", cachedPayload!.Name);
            Assert.Equal("Wondrous Item", cachedPayload.Type);
        }

        [Fact]
        public async Task GetItems_MissThenHit_ExternalCalledOnce_AndCacheWritten()
        {
            FakeDistributedCache cache = new FakeDistributedCache();

            FakeOpen5eItemClient client = new FakeOpen5eItemClient
            {
                ListResponse = new Open5eItemListResponse
                {
                    Count = 2,
                    Results = new List<Open5eItemListItem>
                    {
                        new Open5eItemListItem
                        {
                            Slug = "flame-tongue",
                            Name = "Flame Tongue",
                            Type = "Weapon",
                            Rarity = "rare",
                            RequiresAttunement = "yes"
                        },
                        new Open5eItemListItem
                        {
                            Slug = "frost-brand",
                            Name = "Frost Brand",
                            Type = "Weapon",
                            Rarity = "very rare",
                            RequiresAttunement = "yes"
                        }
                    }
                }
            };

            ItemsService service = new ItemsService(client, cache);

            ItemListResponse first = await service.GetItems(" Sword ", 10, CancellationToken.None);
            ItemListResponse second = await service.GetItems("sword", 10, CancellationToken.None);

            Assert.Equal(2, first.Count);
            Assert.Equal(2, second.Count);
            Assert.Equal(2, first.Results.Count);
            Assert.Equal(2, second.Results.Count);

            Assert.Equal(1, client.FetchItemListCallCount);
            Assert.Equal("sword", client.LastListName);
            Assert.Equal(10, client.LastListLimit);

            Assert.Equal(1, cache.SetCallCount);
            Assert.Equal("dmcomp:items:list:name:sword:limit:10", cache.LastSetKey);
            Assert.NotNull(cache.LastSetOptions);
            Assert.Equal(TimeSpan.FromMinutes(10), cache.LastSetOptions!.AbsoluteExpirationRelativeToNow);

            string? cachedJson = cache.GetStoredString("dmcomp:items:list:name:sword:limit:10");
            Assert.NotNull(cachedJson);

            ItemListResponse? cachedPayload = JsonSerializer.Deserialize<ItemListResponse>(cachedJson!);
            Assert.NotNull(cachedPayload);
            Assert.Equal(2, cachedPayload!.Count);
            Assert.Equal(2, cachedPayload.Results.Count);
            Assert.Equal("Flame Tongue", cachedPayload.Results[0].Name);
        }
    }
}