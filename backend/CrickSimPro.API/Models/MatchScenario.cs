namespace CrickSimPro.API.Models
{
    public class MatchScenario
    {
        public string PitchType { get; set; }
        public string Weather { get; set; }
        public int BattingAggression { get; set; }
        public List<string> BowlerTypes { get; set; }
        public int BowlingAggression { get; set; }
        public int Overs { get; set; }
        public string GameType { get; set; }
        public int CurrentDay { get; set; }
        public List<BatterProfile> Batters { get; set; }
        public int? TargetScore { get; set; }

        // If you want to swap teams for second innings, you need a "BattingSecond" property (add this):
        public MatchScenario BattingSecond { get; set; } // optional, set to null for single innings

        // Helper method to clone and set up second innings scenario:
        public MatchScenario CloneForSecondInnings(int targetScore)
        {
            // Assume BattingSecond is set with the correct team, bowlers, etc.
            return new MatchScenario
            {
                PitchType = this.PitchType,
                Weather = this.Weather,
                BattingAggression = BattingSecond?.BattingAggression ?? this.BattingAggression,
                BowlerTypes = BattingSecond?.BowlerTypes?.ToList() ?? this.BowlerTypes.ToList(),
                BowlingAggression = BattingSecond?.BowlingAggression ?? this.BowlingAggression,
                Overs = this.Overs,
                GameType = this.GameType,
                CurrentDay = this.CurrentDay,
                Batters = BattingSecond?.Batters?.Select(b => new BatterProfile { Name = b.Name, Type = b.Type }).ToList() ?? new List<BatterProfile>(),
                TargetScore = targetScore,
                BattingSecond = null // No further innings for the second team
            };
        }
    }
}
