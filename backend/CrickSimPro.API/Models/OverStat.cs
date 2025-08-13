using System.Collections.Generic;

namespace CrickSimPro.API.Models
{
    public class OverStat
    {
        public int OverNumber { get; set; }
        public string? Bowler { get; set; }
        public List<string>? Deliveries { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
    }
}
