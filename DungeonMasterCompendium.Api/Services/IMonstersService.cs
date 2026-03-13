using DungeonMasterCompendium.Api.Integrations.Open5e;

namespace DungeonMasterCompendium.Api.Services
{
    public interface IMonstersService
    {
        public Task<Open5eMonsterListResponse> GetMonsters(string? name, int limit, CancellationToken cancellationToken);
    }
}
