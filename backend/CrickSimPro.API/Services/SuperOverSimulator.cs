using CrickSimPro.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrickSimPro.API.Services
{
    public class SuperOverSimulator
    {
        public SimulationResult SimulateSuperOver(
            MatchScenario scenario, List<PlayerProfile> battingTeam, List<PlayerProfile> bowlingTeam, Random random)
        {
            int totalRuns = 0, totalWickets = 0;
            int balls = 6;
            int strikerIndex = 0, nonStrikerIndex = 1, nextBatterIndex = 2;
            string striker = battingTeam[strikerIndex].Name;
            string nonStriker = battingTeam[nonStrikerIndex].Name;

            var bowlers = bowlingTeam.Where(p => !string.IsNullOrWhiteSpace(p.BowlingType) && p.BowlingType != "None").ToList();
            var bowler = bowlers[random.Next(bowlers.Count)];
            string bowlerType = bowler.BowlingType;

            var overStatsList = new List<OverStat>();
            var currentOver = new List<string>();

            for (int ball = 1; ball <= balls; ball++)
            {
                if (totalWickets >= 2) break; // Only 2 wickets allowed in super over

                int outcome = random.Next(0, 8);
                if (outcome == 7)
                {
                    totalWickets++;
                    currentOver.Add($"{striker}: W");
                    if (nextBatterIndex < battingTeam.Count)
                    {
                        striker = battingTeam[nextBatterIndex].Name;
                        nextBatterIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    totalRuns += outcome;
                    currentOver.Add($"{striker}: {outcome}");
                    if (outcome % 2 == 1)
                        (striker, nonStriker) = (nonStriker, striker);
                }
            }

            overStatsList.Add(new OverStat
            {
                OverNumber = 1,
                Bowler = $"{bowler.Name} ({bowlerType})",
                Deliveries = new List<string>(currentOver),
                Runs = totalRuns,
                Wickets = totalWickets
            });

            return new SimulationResult
            {
                Message = "Super Over",
                Pitch = scenario.PitchType,
                Weather = scenario.Weather,
                Runs = totalRuns,
                Wickets = totalWickets,
                OversDetail = new List<List<string>> { currentOver },
                OverStats = overStatsList,
                IsChase = false,
                TargetScore = null
            };
        }
    }
}
