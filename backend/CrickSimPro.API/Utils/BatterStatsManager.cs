using CrickSimPro.API.Models;

namespace CrickSimPro.Utils
{
    public static class BatterStatsManager
    {
        private static readonly Dictionary<string, BatterStats> Stats = new();

        public static void Initialize(List<BatterProfile> batters)
        {
            Stats.Clear();
            foreach (var batter in batters)
            {
                Stats[batter.Name] = new BatterStats
                {
                    Name = batter.Name,
                    Type = batter.Type,
                    Runs = 0,
                    BallsFaced = 0,
                    IsOut = false
                };
            }
        }

        public static void RecordBall(string batterName, string outcome)
        {
            if (!Stats.ContainsKey(batterName)) return;
            var batter = Stats[batterName];

            if (outcome == "W")
            {
                batter.IsOut = true;
            }
            else
            {
                if (int.TryParse(outcome, out int runs))
                {
                    batter.Runs += runs;
                    batter.BallsFaced++;
                }
            }
        }

        public static List<BatterStats> GetAllStats()
        {
            return Stats.Values.ToList();
        }
    }
}
