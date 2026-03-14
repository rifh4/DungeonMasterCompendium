namespace DungeonMasterCompendium.Api.Integrations.Open5e.Spells
{
    public interface IOpen5eSpellClient
    {
        Task<Open5eSpellListResponse> FetchSpellList(string? name, int limit, CancellationToken cancellationToken);
        Task<Open5eSpellDetailItem?> FetchSpellDetails(string externalId, CancellationToken cancellationToken);
    }
}
