namespace DungeonMasterCompendium.Api.Contracts.Monsters
{
    public sealed class MonsterListItemResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Alignment {  get; set; } = string.Empty;
        public string ChallengeRating {  get; set; } = string.Empty;

    }
}
