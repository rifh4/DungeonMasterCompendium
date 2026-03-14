using System.Text.Json.Serialization;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Items
{
    public sealed class Open5eItemListItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;

        [JsonPropertyName("requires_attunement")]
        public string RequiresAttunement { get; set; } = string.Empty;
    }
}