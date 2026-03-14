namespace DungeonMasterCompendium.Api.Integrations.Open5e.Monsters
{
    public class Open5eMonsterListResponse
    {
        public int Count { get; set; }
        public string? Next {  get; set; }
        public string? Previous { get; set; }
        public List<Open5eMonsterListItem> Results { get; set; } = new();
    }
}
