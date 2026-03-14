using System.Text.Json.Serialization;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Monsters
{
    public class Open5eMonsterListItem
    {
        public string Slug {  get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Alignment { get; set; } = string.Empty;


        [JsonPropertyName("challenge_rating")]
        public string ChallengeRating { get; set; } = string.Empty;
    }
}
