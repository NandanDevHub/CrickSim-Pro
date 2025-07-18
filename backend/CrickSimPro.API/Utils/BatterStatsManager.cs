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

        // Always counts the ball faced, whether runs or wicket
        public static void RecordBall(string batterName, string outcome)
        {
            if (!Stats.ContainsKey(batterName)) return;
            var batter = Stats[batterName];
            batter.BallsFaced++;

            if (outcome == "W")
            {
                batter.IsOut = true;
            }
            else if (int.TryParse(outcome, out int runs))
            {
                batter.Runs += runs;
            }
        }

        public static List<BatterStats> GetAllStats()
        {
            return Stats.Values.ToList();
        }

        public static void Clear()
        {
            Stats.Clear();
        }
    }
}
