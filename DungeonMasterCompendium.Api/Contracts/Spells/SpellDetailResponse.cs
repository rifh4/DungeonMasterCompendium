namespace DungeonMasterCompendium.Api.Contracts.Spells
{
    public sealed class SpellDetailResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string CastingTime { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HigherLevel { get; set; } = string.Empty;
        public string Components { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Ritual { get; set; } = string.Empty;
    }
}
