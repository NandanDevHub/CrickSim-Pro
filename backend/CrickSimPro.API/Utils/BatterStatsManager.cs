using CrickSimPro.API.Models;
using CrickSimPro.Constants;
using System.Collections.Generic;
using System.Linq;

namespace CrickSimPro.Utils
{
    public static class BatterStatsManager
    {
        private static readonly Dictionary<string, BatterStats> Stats = [];

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
                    HowOut = null,
                    RetiredHurt = false,
                    DidNotBat = false,
                    Fours = 0,
                    Sixes = 0,
                    Extras = 0
                };
            }
        }

        public static void RecordBall(string batterName, string outcome, string? howOut = null)
        {
            if (!Stats.ContainsKey(batterName)) return;
            var batter = Stats[batterName];

            if (batter.DidNotBat) return;

            if (outcome == SimulationConstants.Wicket)
            {
                batter.IsOut = true;
                batter.BallsFaced++;
                batter.HowOut = howOut;
            }
            else if (outcome == SimulationConstants.RetiredHurt)
            {
                batter.RetiredHurt = true;
            }
            else if (outcome == SimulationConstants.Wide || outcome == SimulationConstants.NoBall || outcome == SimulationConstants.Byes || outcome == SimulationConstants.LegByes)
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
            return [.. Stats.Values];
        }
    }
}
