namespace DungeonMasterCompendium.Api.Contracts.Items
{
    public sealed class ItemListItemResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string RequiresAttunement { get; set; } = string.Empty;
    }
}