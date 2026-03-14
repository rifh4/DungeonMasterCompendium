using System.Text.Json.Serialization;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Spells
{
    public sealed class Open5eSpellDetailItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;

        [JsonPropertyName("casting_time")]
        public string CastingTime { get; set; } = string.Empty;

        public string Range { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;

        [JsonPropertyName("higher_level")]
        public string HigherLevel { get; set; } = string.Empty;

        public string Components { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Ritual { get; set; } = string.Empty;
    }
}
