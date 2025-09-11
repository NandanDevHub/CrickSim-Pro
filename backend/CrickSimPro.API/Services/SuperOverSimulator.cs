using CrickSimPro.API.Models;
using CrickSimPro.Constants;
using CrickSimPro.Utils;
using System;
using System.Collections.Generic;

namespace CrickSimPro.API.Services
{
    public class SuperOverSimulator
    {
        public SimulationResult SimulateSuperOver(
            MatchScenario scenario,
            List<PlayerProfile> battingTeam,
            List<PlayerProfile> bowlingTeam,
            Random rng)
        {
            int balls = 6;
            int totalRuns = 0;
            int totalWickets = 0;
            var allOvers = new List<List<string>>();
            var overStatsList = new List<OverStat>();

            var bowlingCandidates = bowlingTeam
                .FindAll(p => !string.IsNullOrWhiteSpace(p.BowlingType) && p.BowlingType != "None");
            var bowlerOversCount = bowlingCandidates.ToDictionary(b => b.Name, _ => 0);

            BowlingRotationManager.Initialize(bowlingCandidates.ConvertAll(b => b.Name));
            BowlerStatsManager.Initialize(bowlingCandidates);
            BatterStatsManager.Initialize(battingTeam);
            PlayerStaminaManager.InitializeStamina(
                battingTeam.ConvertAll(p => new BatterProfile { Name = p.Name, Type = p.BattingType }),
                bowlingCandidates.ConvertAll(b => b.Name));

            int strikerIndex = 0, nonStrikerIndex = 1, nextBatterIndex = 2;
            string striker = battingTeam[strikerIndex].Name, nonStriker = battingTeam[nonStrikerIndex].Name;
            var recentRuns = new Dictionary<string, Queue<int>>
            {
                [striker] = new Queue<int>(),
                [nonStriker] = new Queue<int>()
            };

            string? lastBowler = null;
            var currentOver = new List<string>();
            int runsThisOver = 0;
            int wicketsThisOver = 0;

            var currentBowler = bowlingCandidates[rng.Next(bowlingCandidates.Count)];
            lastBowler = currentBowler.Name;

            for (int ball = 1; ball <= balls; ball++)
            {
                int pressure = 0;
                int batterStamina = PlayerStaminaManager.GetBatterStamina(striker);
                double modifier = PlayerStaminaManager.GetPerformanceModifierFromStamina(batterStamina);
                int adjustedAggression = (int)(scenario.BattingAggression * modifier);

                int matchupModifier = 0;
                var batterType = battingTeam.Find(b => b.Name == striker).BattingType;
                var bowlerTypeCheck = currentBowler.BowlingType.ToLower();
                if (batterType == "Anchor" && bowlerTypeCheck == "spin")
                    matchupModifier += 3;
                if (batterType == "Aggressive" && bowlerTypeCheck == "swing")
                    matchupModifier += 2;
                if (batterType == "Finisher" && bowlerTypeCheck == "spin")
                    matchupModifier += 4;
                adjustedAggression += matchupModifier;

                int bowlerAggression = scenario.BowlingAggression;

                var (outcome, howOut) = MatchSimulationHelper.SimulateBallWithWicketMode(
                    adjustedAggression, bowlerAggression, scenario.GameType,
                    scenario.PitchType, scenario.Weather, currentBowler.BowlingType,
                    scenario.CurrentDay, 1, pressure, PlayerStaminaManager.GetBowlerStamina(currentBowler.Name));

                if (outcome == SimulationConstants.Wide || outcome == SimulationConstants.NoBall || outcome == SimulationConstants.Byes || outcome == SimulationConstants.LegByes)
                {
                    currentOver.Add($"{striker}: {outcome}");
                    BatterStatsManager.RecordBall(striker, outcome);
                    BowlerStatsManager.RecordDelivery(currentBowler.Name, outcome);
                    runsThisOver++;
                    totalRuns++;
                    ball--;
                    continue;
                }

                if (outcome == SimulationConstants.Wicket)
                {
                    totalWickets++;
                    wicketsThisOver++;
                    currentOver.Add($"{striker}: {howOut}");
                    BatterStatsManager.RecordBall(striker, SimulationConstants.Wicket, howOut);
                    BowlerStatsManager.RecordDelivery(currentBowler.Name, SimulationConstants.Wicket, howOut);

                    if (nextBatterIndex < battingTeam.Count)
                    {
                        striker = battingTeam[nextBatterIndex].Name;
                        nextBatterIndex++;
                        recentRuns[striker] = new Queue<int>();
                    }
                    else
                    {
                        currentOver.Add("No batters left. All Out.");
                        break;
                    }
                }
                else
                {
                    currentOver.Add($"{striker}: {outcome}");
                    BatterStatsManager.RecordBall(striker, outcome);
                    BowlerStatsManager.RecordDelivery(currentBowler.Name, outcome);

                    int run = int.Parse(outcome);
                    if (!recentRuns.ContainsKey(striker))
                        recentRuns[striker] = new Queue<int>();
                    recentRuns[striker].Enqueue(run);
                    if (recentRuns[striker].Count > 6)
                        recentRuns[striker].Dequeue();

                    totalRuns += run;
                    runsThisOver += run;

                    PlayerStaminaManager.ReduceBatterStamina(striker, run, pressure);

                    if (run % 2 == 1)
                        (nonStriker, striker) = (striker, nonStriker);
                }
            }

            allOvers.Add(currentOver);
            overStatsList.Add(new OverStat
            {
                OverNumber = 1,
                Bowler = $"{currentBowler.Name} ({currentBowler.BowlingType})",
                Deliveries = [.. currentOver],
                Runs = runsThisOver,
                Wickets = wicketsThisOver
            });

            return new SimulationResult
            {
                Message = $"Super Over Completed",
                Pitch = scenario.PitchType,
                Weather = scenario.Weather,
                Runs = totalRuns,
                Wickets = totalWickets,
                OversDetail = allOvers,
                OverStats = overStatsList,
                BatterStats = BatterStatsManager.GetAllStats(),
                BowlerStats = BowlerStatsManager.GetAllStats(),
                IsChase = false,
                TargetScore = null
            };
        }
    }
}
