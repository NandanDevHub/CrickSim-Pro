namespace CrickSimPro.API.Models
{
    public class SimulationResult
    {
        public required string Message { get; set; }
        public required string Pitch { get; set; }
        public required string Weather { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
        public List<List<string>>? OversDetail { get; set; }
        public List<OverStat>? OverStats { get; set; }
        public List<BatterStats>? BatterStats { get; set; }
        public List<BowlerStats>? BowlerStats { get; set; }
        public bool IsChase { get; set; }
        public int? TargetScore { get; set; }
    }
}
