namespace DungeonMasterCompendium.Api.Integrations.Open5e.Spells
{
    public sealed class Open5eSpellListResponse
    {
        public int Count { get; set; }
        public List<Open5eSpellListItem> Results { get; set; } = new List<Open5eSpellListItem>();
    }
}
