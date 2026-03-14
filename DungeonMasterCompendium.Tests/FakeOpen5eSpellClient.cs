using DungeonMasterCompendium.Api.Integrations.Open5e.Spells;

namespace DungeonMasterCompendium.Tests
{
    public class FakeOpen5eSpellClient : IOpen5eSpellClient
    {
        public int FetchSpellListCallCount { get; private set; }
        public int FetchSpellDetailsCallCount { get; private set; }

        public string? LastListName { get; private set; }
        public int LastListLimit { get; private set; }
        public string? LastDetailExternalId { get; private set; }

        public Open5eSpellListResponse ListResponse { get; set; } = new Open5eSpellListResponse();
        public Open5eSpellDetailItem? DetailResponse { get; set; }

        public Task<Open5eSpellListResponse> FetchSpellList(string? name, int limit, CancellationToken cancellationToken)
        {
            FetchSpellListCallCount++;
            LastListName = name;
            LastListLimit = limit;

            return Task.FromResult(ListResponse);
        }

        public Task<Open5eSpellDetailItem?> FetchSpellDetails(string externalId, CancellationToken cancellationToken)
        {
            FetchSpellDetailsCallCount++;
            LastDetailExternalId = externalId;

            return Task.FromResult(DetailResponse);
        }
    }
}