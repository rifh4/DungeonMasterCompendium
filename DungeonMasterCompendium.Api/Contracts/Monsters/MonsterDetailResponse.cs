namespace DungeonMasterCompendium.Api.Contracts.Monsters
{
    public class MonsterDetailResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Alignment { get; set; } = string.Empty;
        public int ArmorClass {  get; set; } 
        public int HitPoints { get; set; }
        public string HitDice {  get; set; } = string.Empty;
        public string Speed {  get; set; } = string.Empty;
        public int Strength {  get; set; }
        public int Dexterity { get; set; } 
        public int Constitution { get; set; } 
        public int Intelligence { get; set; } 
        public int Wisdom { get; set; } 
        public int Charisma { get; set; } 
        public string ChallengeRating { get; set; } = string.Empty;
    }
}
