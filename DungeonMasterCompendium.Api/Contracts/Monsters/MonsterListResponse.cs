namespace DungeonMasterCompendium.Api.Contracts.Monsters
{
    public sealed class MonsterListResponse
    {
        public int Count { get; set; }
        public List<MonsterListItemResponse> Results { get; set; } = new();
    }
}
