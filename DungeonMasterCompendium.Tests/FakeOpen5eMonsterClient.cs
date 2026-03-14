using DungeonMasterCompendium.Api.Integrations.Open5e.Monsters;

namespace DungeonMasterCompendium.Tests
{
    public class FakeOpen5eMonsterClient : IOpen5eMonsterClient
    {
        public int FetchMonsterListCallCount { get; private set; }
        public int FetchMonsterDetailsCallCount { get; private set; }

        public string? LastListName { get; private set; }
        public int LastListLimit { get; private set; }
        public string? LastDetailExternalId { get; private set; }

        public Open5eMonsterListResponse ListResponse { get; set; } = new Open5eMonsterListResponse();
        public Open5eMonsterDetailItem? DetailResponse { get; set; }

        public Task<Open5eMonsterListResponse> FetchMonsterList(string? name, int limit, CancellationToken cancellationToken)
        {
            FetchMonsterListCallCount++;
            LastListName = name;
            LastListLimit = limit;

            return Task.FromResult(ListResponse);
        }

        public Task<Open5eMonsterDetailItem?> FetchMonsterDetails(string externalId, CancellationToken cancellationToken)
        {
            FetchMonsterDetailsCallCount++;
            LastDetailExternalId = externalId;

            return Task.FromResult(DetailResponse);
        }
    }
}
