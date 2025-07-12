namespace CrickSimPro.API.Models
{
    public class BatterStats
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Runs { get; set; }
        public int BallsFaced { get; set; }
        public bool IsOut { get; set; }

        public double StrikeRate => BallsFaced == 0 ? 0 : (Runs * 100.0) / BallsFaced;
    }
}
