namespace CrickSimPro.API.Models
{
    public class SimulationResult
    {
        public string Message { get; set; }
        public string Pitch { get; set; }
        public string Weather { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
        public List<List<string>> OversDetail { get; set; }
        public List<OverStat> OverStats { get; set; }
        public List<BatterStats> BatterStats { get; set; }
        public List<BowlerStats> BowlerStats { get; set; }
        public bool IsChase { get; set; }         // true if this is a chase/second innings
        public int? TargetScore { get; set; }     // target, if chasing
    }
}
