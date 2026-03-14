namespace DungeonMasterCompendium.Api.Contracts.Spells
{
    public sealed class SpellListResponse
    {
        public int Count { get; set; }
        public List<SpellListItemResponse> Results { get; set; } = new List<SpellListItemResponse>();
    }
}
