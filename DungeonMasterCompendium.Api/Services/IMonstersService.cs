using DungeonMasterCompendium.Api.Contracts.Monsters;

namespace DungeonMasterCompendium.Api.Services
{
    public interface IMonstersService
    {
        Task<MonsterListResponse> GetMonsters(string? name, int limit, CancellationToken cancellationToken);
        Task<MonsterDetailResponse?> GetMonsterDetails(string externalId, CancellationToken cancellationToken);
    }
}