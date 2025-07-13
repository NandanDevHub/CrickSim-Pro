using CrickSimPro.API.Models;

namespace CrickSimPro.Utils
{
    public static class BowlerStatsManager
    {
        private static readonly Dictionary<string, BowlerStats> Stats = new();

        public static void Initialize(List<string> bowlerTypes)
        {
            Stats.Clear();
            foreach (var bowler in bowlerTypes)
            {
                Stats[bowler] = new BowlerStats
                {
                    Name = bowler,
                    Overs = 0,
                    BallsBowled = 0,
                    RunsConceded = 0,
                    Wickets = 0
                };
            }
        }

        public static void RecordDelivery(string bowler, string outcome)
        {
            if (!Stats.TryGetValue(bowler, out BowlerStats? b)) return;
            b.BallsBowled++;

            if (outcome == "W")
                b.Wickets++;
            else if (int.TryParse(outcome, out int run))
                b.RunsConceded += run;

            b.Overs = b.BallsBowled / 6;
        }

        public static List<BowlerStats> GetAllStats()
        {
            return Stats.Values.ToList();
        }
    }
}
