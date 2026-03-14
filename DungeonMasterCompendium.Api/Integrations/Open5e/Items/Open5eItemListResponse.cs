namespace DungeonMasterCompendium.Api.Integrations.Open5e.Items
{
    public sealed class Open5eItemListResponse
    {
        public int Count { get; set; }
        public List<Open5eItemListItem> Results { get; set; } = new List<Open5eItemListItem>();
    }
}