namespace CrickSimPro.API.Models
{
    public class BatterStats
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Runs { get; set; }
        public int BallsFaced { get; set; }
        public bool IsOut { get; set; }
        public bool RetiredHurt { get; set; }
        public bool DidNotBat { get; set; }
        public int Fours { get; set; }
        public int Sixes { get; set; }
        public int Extras { get; set; }
        public double StrikeRate => BallsFaced > 0 ? (double)Runs / BallsFaced * 100 : 0;
        public int DotBalls { get; set; }
        public int BattingPosition { get; set; }  
    }
}
