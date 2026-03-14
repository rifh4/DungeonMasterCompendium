using DungeonMasterCompendium.Api.Contracts.Spells;

namespace DungeonMasterCompendium.Api.Services
{
    public interface ISpellsService
    {
        Task<SpellListResponse> GetSpells(string? name, int limit, CancellationToken cancellationToken);
        Task<SpellDetailResponse?> GetSpellDetails(string externalId, CancellationToken cancellationToken);
    }
}
