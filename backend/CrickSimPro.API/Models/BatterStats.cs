namespace CrickSimPro.API.Models
{
    public class BatterStats
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int Runs { get; set; }
        public int BallsFaced { get; set; }
        public bool IsOut { get; set; }
        public string? HowOut { get; set; }
        public bool RetiredHurt { get; set; }
        public bool DidNotBat { get; set; }
        public int Fours { get; set; }
        public int Sixes { get; set; }
        public int Extras { get; set; }
    }
}
