namespace CrickSimPro.API.Models
{
    public class BowlerStats
    {
        public required string Name { get; set; }
        public double Overs { get; set; }
        public int BallsBowled { get; set; }
        public int RunsConceded { get; set; }
        public int Wickets { get; set; }
        public List<string> WicketModes { get; set; } = [];
    }
}
