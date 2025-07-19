using CrickSimPro.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace CrickSimPro.Utils
{
    public static class BowlerStatsManager
    {
        private static readonly Dictionary<string, BowlerStats> Stats = new();

        // For legacy (list of string)
        public static void Initialize(List<string> bowlerNames)
        {
            Stats.Clear();
            foreach (var bowler in bowlerNames)
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

        // For modern (list of PlayerProfile)
        public static void Initialize(List<PlayerProfile> bowlerPlayers)
        {
            Stats.Clear();
            foreach (var player in bowlerPlayers)
            {
                Stats[player.Name] = new BowlerStats
                {
                    Name = player.Name,
                    Overs = 0,
                    BallsBowled = 0,
                    RunsConceded = 0,
                    Wickets = 0
                };
            }
        }

        public static void RecordDelivery(string bowler, string outcome)
        {
            if (!Stats.TryGetValue(bowler, out var b)) return;

            // Extras (wides, no balls, byes, leg byes): runs conceded but no ball added except on legal deliveries
            if (outcome == "WD" || outcome == "NB" || outcome == "B" || outcome == "LB")
            {
                b.RunsConceded++;
                return;
            }

            b.BallsBowled++;
            if (outcome == "W")
                b.Wickets++;
            else if (int.TryParse(outcome, out int run) && run >= 0 && run <= 6)
                b.RunsConceded += run;

            b.Overs = b.BallsBowled / 6 + (b.BallsBowled % 6) / 10.0;
        }

        public static List<BowlerStats> GetAllStats()
        {
            return Stats.Values.ToList();
        }

        public static void Clear()
        {
            Stats.Clear();
        }
    }
}
