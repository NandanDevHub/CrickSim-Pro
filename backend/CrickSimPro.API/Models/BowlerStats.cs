namespace CrickSimPro.API.Models
{
    public class BowlerStats
    {
        public string Name { get; set; }
        public double Overs { get; set; }
        public int BallsBowled { get; set; }
        public int RunsConceded { get; set; }
        public int Wickets { get; set; }
        public double Economy => BallsBowled == 0 ? 0 : (RunsConceded * 6.0) / BallsBowled;
    }
}
