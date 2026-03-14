using System.Text.Json.Serialization;

namespace DungeonMasterCompendium.Api.Integrations.Open5e.Monsters
{
    public class Open5eMonsterDetailItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Size {  get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Alignment {  get; set; } = string.Empty;

        [JsonPropertyName("armor_class")]
        public int ArmorClass { get; set; }

        [JsonPropertyName("hit_points")]
        public int HitPoints { get; set; }
        [JsonPropertyName("hit_dice")]
        public string HitDice { get; set; } = string.Empty;
        public Dictionary<string, int> Speed {  get; set; } = new Dictionary<string, int>();
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        [JsonPropertyName("challenge_rating")]
        public string ChallengeRating {  get; set; } = string.Empty;
    }
}
