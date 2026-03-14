using DungeonMasterCompendium.Api.Integrations.Open5e.Items;

namespace DungeonMasterCompendium.Tests
{
    public class FakeOpen5eItemClient : IOpen5eItemClient
    {
        public int FetchItemListCallCount { get; private set; }
        public int FetchItemDetailsCallCount { get; private set; }

        public string? LastListName { get; private set; }
        public int LastListLimit { get; private set; }
        public string? LastDetailExternalId { get; private set; }

        public Open5eItemListResponse ListResponse { get; set; } = new Open5eItemListResponse();
        public Open5eItemDetailItem? DetailResponse { get; set; }

        public Task<Open5eItemListResponse> FetchItemList(string? name, int limit, CancellationToken cancellationToken)
        {
            FetchItemListCallCount++;
            LastListName = name;
            LastListLimit = limit;

            return Task.FromResult(ListResponse);
        }

        public Task<Open5eItemDetailItem?> FetchItemDetails(string externalId, CancellationToken cancellationToken)
        {
            FetchItemDetailsCallCount++;
            LastDetailExternalId = externalId;

            return Task.FromResult(DetailResponse);
        }
    }
}