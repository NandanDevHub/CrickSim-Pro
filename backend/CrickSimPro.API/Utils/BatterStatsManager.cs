using CrickSimPro.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace CrickSimPro.Utils
{
    public static class BatterStatsManager
    {
        private static readonly Dictionary<string, BatterStats> Stats = new();

        public static void Initialize(List<PlayerProfile> batters)
        {
            Stats.Clear();
            foreach (var batter in batters)
            {
                Stats[batter.Name] = new BatterStats
                {
                    Name = batter.Name,
                    Type = batter.BattingType,
                    Runs = 0,
                    BallsFaced = 0,
                    IsOut = false,
                    RetiredHurt = false,
                    DidNotBat = false,
                    Fours = 0,
                    Sixes = 0,
                    Extras = 0
                };
            }
        }

        public static void RetireHurt(string batterName)
        {
            if (!Stats.ContainsKey(batterName)) return;
            Stats[batterName].RetiredHurt = true;
        }

        public static void SetDidNotBat(string batterName)
        {
            if (!Stats.ContainsKey(batterName)) return;
            Stats[batterName].DidNotBat = true;
        }

        public static void RecordBall(string batterName, string outcome)
        {
            if (!Stats.ContainsKey(batterName)) return;
            var batter = Stats[batterName];

            if (batter.DidNotBat) return;

            if (outcome == "W")
            {
                batter.IsOut = true;
                batter.BallsFaced++;
            }
            else if (outcome == "RetiredHurt")
            {
                batter.RetiredHurt = true;
            }
            else if (outcome == "WD" || outcome == "NB" || outcome == "B" || outcome == "LB")
            {
                batter.Extras++;
            }
            else if (int.TryParse(outcome, out int runs) && runs >= 0 && runs <= 6)
            {
                batter.BallsFaced++;
                batter.Runs += runs;
                if (runs == 4) batter.Fours++;
                if (runs == 6) batter.Sixes++;
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
