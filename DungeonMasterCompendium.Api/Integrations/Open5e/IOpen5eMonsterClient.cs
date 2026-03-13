namespace DungeonMasterCompendium.Api.Integrations.Open5e
{
    interface IOpen5eMonsterClient
    {
        public Task<Open5eMonsterListResponse> FetchMonsterList(string? name, int limit, CancellationToken cancellationToken);
        public Task<Open5eMonsterDetailItem?> FetchMonsterDetails(string externalId, CancellationToken cancellationToken);
    }
}
