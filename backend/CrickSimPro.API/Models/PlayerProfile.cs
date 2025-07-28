namespace CrickSimPro.API.Models
{
    public class PlayerProfile
    {
        public string Name { get; set; }
        public string BattingType { get; set; }
        public string BowlingType { get; set; }
        public int BattingSkill { get; set; }
        public int BowlingSkill { get; set; }
        public int Stamina { get; set; }
        // Extend as needed: e.g., IsWicketKeeper, FieldingSkill, etc.
    }
}
