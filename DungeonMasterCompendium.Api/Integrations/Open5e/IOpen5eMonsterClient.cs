namespace DungeonMasterCompendium.Api.Integrations.Open5e
{
    public interface IOpen5eMonsterClient
    {
         Task<Open5eMonsterListResponse> FetchMonsterList(string? name, int limit, CancellationToken cancellationToken);
         Task<Open5eMonsterDetailItem?> FetchMonsterDetails(string externalId, CancellationToken cancellationToken);
    }
}
