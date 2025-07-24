namespace CrickSimPro.API.Models
{
    public class PlayerProfile
    {
        public string Name { get; set; }
        public string BattingType { get; set; }    // e.g. "Aggressive", "Anchor", "Finisher", "AllRounder", "Tailender"
        public string BowlingType { get; set; }    // e.g. "Swing", "Spin", "Aggressive", "AllRounder", "None"
        public int BattingSkill { get; set; }      // 1-100 scale, can be used for advanced sim
        public int BowlingSkill { get; set; }      // 1-100 scale
        public int Stamina { get; set; }           // Optional, default = 100
        // Extend as needed: e.g., IsWicketKeeper, FieldingSkill, etc.
    }
}
