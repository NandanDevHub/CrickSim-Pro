using CrickSimPro.API.Models;
using CrickSimPro.Constants;
using CrickSimPro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrickSimPro.API.Services
{
    public class FullMatchResult
    {
        public SimulationResult FirstInningsResult { get; set; }
        public SimulationResult SecondInningsResult { get; set; }
        public SimulationResult ThirdInningsResult { get; set; }
        public SimulationResult FourthInningsResult { get; set; }
        public string MatchSummary { get; set; }
        public string MatchResultType { get; set; }
        public string Winner { get; set; }
        public int? Margin { get; set; }

        public SimulationResult SuperOverResultTeam1 { get; set; }
        public SimulationResult SuperOverResultTeam2 { get; set; }

    }

    public class MatchSimulator
    {
        private readonly Random _random = new();

        public FullMatchResult Simulate(MatchScenario scenario)
        {
            if (scenario.GameType == SimulationConstants.GameTypeTest)
            {
                var firstInningsResult = SimulateInnings(scenario, false, null);

                var secondScenario = scenario.CloneForSecondInnings(firstInningsResult.Runs + 1);
                var secondInningsResult = SimulateInnings(secondScenario, true, null);

                bool enforceFollowOn = false;
                int followOnThreshold = 200;
                int team1Score = firstInningsResult.Runs;
                int team2Score = secondInningsResult.Runs;
                if ((team1Score - team2Score) >= followOnThreshold)
                {
                    enforceFollowOn = true;
                }

                SimulationResult thirdInningsResult = null;
                SimulationResult fourthInningsResult = null;
                string summary = "", resultType = "", winner = ""; int? margin = null;

                if (enforceFollowOn)
                {
                    var thirdScenario = scenario.CloneForThirdInnings(0, forceTeam2Batting: true);
                    thirdInningsResult = SimulateInnings(thirdScenario, true, null);

                    int testTarget = (team1Score + 1) - (team2Score + thirdInningsResult.Runs);
                    var fourthScenario = scenario.CloneForFourthInnings(testTarget);
                    fourthInningsResult = SimulateInnings(fourthScenario, false, testTarget);

                    if (fourthInningsResult.Runs >= testTarget)
                    {
                        summary = $"Team 1 won by {10 - fourthInningsResult.Wickets} wickets";
                        resultType = "Win";
                        winner = scenario.BattingFirst;
                        margin = 10 - fourthInningsResult.Wickets;
                    }
                    else if (fourthInningsResult.Wickets >= 10)
                    {
                        summary = $"Team 2 won by {testTarget - fourthInningsResult.Runs - 1} runs";
                        resultType = "Win";
                        winner = scenario.BattingSecond;
                        margin = testTarget - fourthInningsResult.Runs - 1;
                    }
                    else if ((fourthInningsResult.Runs == testTarget - 1) && fourthInningsResult.Wickets >= 10)
                    {
                        summary = "Match Tied";
                        resultType = "Tie";
                    }
                    else
                    {
                        summary = "Match Drawn";
                        resultType = "Draw";
                    }
                }
                else
                {
                    var thirdScenario = scenario.CloneForThirdInnings(team1Score - team2Score);
                    thirdInningsResult = SimulateInnings(thirdScenario, false, null);

                    int testTarget = (team1Score + thirdInningsResult.Runs) - team2Score + 1;
                    var fourthScenario = scenario.CloneForFourthInnings(testTarget);
                    fourthInningsResult = SimulateInnings(fourthScenario, true, testTarget);

                    if (fourthInningsResult.Runs >= testTarget)
                    {
                        summary = $"Team 2 won by {10 - fourthInningsResult.Wickets} wickets";
                        resultType = "Win";
                        winner = scenario.BattingSecond;
                        margin = 10 - fourthInningsResult.Wickets;
                    }
                    else if (fourthInningsResult.Wickets >= 10)
                    {
                        summary = $"Team 1 won by {testTarget - fourthInningsResult.Runs - 1} runs";
                        resultType = "Win";
                        winner = scenario.BattingFirst;
                        margin = testTarget - fourthInningsResult.Runs - 1;
                    }
                    else if ((fourthInningsResult.Runs == testTarget - 1) && fourthInningsResult.Wickets >= 10)
                    {
                        summary = "Match Tied";
                        resultType = "Tie";
                    }
                    else
                    {
                        summary = "Match Drawn";
                        resultType = "Draw";
                    }
                }

                return new FullMatchResult
                {
                    FirstInningsResult = firstInningsResult,
                    SecondInningsResult = secondInningsResult,
                    ThirdInningsResult = thirdInningsResult,
                    FourthInningsResult = fourthInningsResult,
                    MatchSummary = summary,
                    MatchResultType = resultType,
                    Winner = winner,
                    Margin = margin
                };
            }
            else
            {
                var firstInningsResult = SimulateInnings(scenario, false, null);
                firstInningsResult.IsChase = false;
                firstInningsResult.TargetScore = null;

                var chaseScenario = scenario.CloneForSecondInnings(firstInningsResult.Runs + 1);
                var secondInningsResult = SimulateInnings(chaseScenario, true, firstInningsResult.Runs + 1);
                secondInningsResult.IsChase = true;
                secondInningsResult.TargetScore = firstInningsResult.Runs + 1;

                string summary = "";
                string resultType = "";
                string winner = "";
                int? margin = null;
                SimulationResult superOverResultTeam1 = null;
                SimulationResult superOverResultTeam2 = null;

                if (secondInningsResult.Runs >= secondInningsResult.TargetScore)
                {
                    summary = $"Team 2 chased successfully with {10 - secondInningsResult.Wickets} wickets left";
                    resultType = "Win";
                    winner = scenario.BattingSecond;
                    margin = 10 - secondInningsResult.Wickets;
                }
                else if (secondInningsResult.Runs == firstInningsResult.Runs)
                {
                    // Adding Super Over Logic
                    var superOverSim = new SuperOverSimulator();

                    // Team 1 bats first in super over
                    var superOverScenario1 = scenario.CloneForSuperOver(isTeamA: true);
                    superOverResultTeam1 = superOverSim.SimulateSuperOver(superOverScenario1, scenario.TeamAPlayers, scenario.TeamBPlayers, _random);

                    // Team 2 bats second in super over
                    var superOverScenario2 = scenario.CloneForSuperOver(isTeamA: false);
                    superOverResultTeam2 = superOverSim.SimulateSuperOver(superOverScenario2, scenario.TeamBPlayers, scenario.TeamAPlayers, _random);

                    if (superOverResultTeam1.Runs > superOverResultTeam2.Runs)
                    {
                        summary = "Team 1 won in Super Over";
                        resultType = "Super Over Win";
                        winner = scenario.BattingFirst;
                        margin = superOverResultTeam1.Runs - superOverResultTeam2.Runs;
                    }
                    else if (superOverResultTeam2.Runs > superOverResultTeam1.Runs)
                    {
                        summary = "Team 2 won in Super Over";
                        resultType = "Super Over Win";
                        winner = scenario.BattingSecond;
                        margin = superOverResultTeam2.Runs - superOverResultTeam1.Runs;
                    }
                    else
                    {
                        summary = "Match and Super Over Tied";
                        resultType = "Tie";
                    }
                }
                else
                {
                    summary = $"Team 1 won by {secondInningsResult.TargetScore - secondInningsResult.Runs - 1} runs";
                    resultType = "Win";
                    winner = scenario.BattingFirst;
                    margin = secondInningsResult.TargetScore - secondInningsResult.Runs - 1;
                }

                return new FullMatchResult
                {
                    FirstInningsResult = firstInningsResult,
                    SecondInningsResult = secondInningsResult,
                    ThirdInningsResult = null,
                    FourthInningsResult = null,
                    SuperOverResultTeam1 = superOverResultTeam1,
                    SuperOverResultTeam2 = superOverResultTeam2,
                    MatchSummary = summary,
                    MatchResultType = resultType,
                    Winner = winner,
                    Margin = margin
                };
            }
        }

        private SimulationResult SimulateInnings(MatchScenario scenario, bool isSecondInnings, int? targetScore)
        {
            int totalOvers = scenario.Overs > 0 ? scenario.Overs : MatchSimulationHelper.GetDefaultOvers(scenario.GameType);
            var allOvers = new List<List<string>>();
            var overStatsList = new List<OverStat>();
            int totalRuns = 0;
            int totalWickets = 0;

            var battingTeam = isSecondInnings ? scenario.TeamBPlayers : scenario.TeamAPlayers;
            var bowlingTeam = isSecondInnings ? scenario.TeamAPlayers : scenario.TeamBPlayers;

            if (battingTeam == null || battingTeam.Count < 2)
                throw new Exception("At least 2 batters required.");
            if (bowlingTeam == null || bowlingTeam.Count == 0)
                throw new Exception("At least 1 bowler required.");

            var bowlingCandidates = bowlingTeam
                .Where(p => !string.IsNullOrWhiteSpace(p.BowlingType) && p.BowlingType != "None")
                .ToList();

            if (bowlingCandidates.Count == 0)
                throw new Exception("No valid bowlers found for fielding team.");

            int maxOversPerBowler =
                scenario.GameType == SimulationConstants.GameTypeT20 ? 4 :
                scenario.GameType == SimulationConstants.GameTypeODI ? 10 :
                totalOvers;

            var bowlerOversCount = bowlingCandidates.ToDictionary(b => b.Name, _ => 0);

            BowlingRotationManager.Initialize(bowlingCandidates.Select(b => b.Name).ToList());
            BowlerStatsManager.Initialize(bowlingCandidates.Select(b => b.Name).ToList());
            BatterStatsManager.Initialize(battingTeam);
            PlayerStaminaManager.InitializeStamina(
                battingTeam.Select(p => new BatterProfile { Name = p.Name, Type = p.BattingType }).ToList(),
                bowlingCandidates.Select(b => b.Name).ToList());

            int strikerIndex = 0, nonStrikerIndex = 1, nextBatterIndex = 2;
            string striker = battingTeam[strikerIndex].Name, nonStriker = battingTeam[nonStrikerIndex].Name;
            var recentRuns = new Dictionary<string, Queue<int>>
            {
                [striker] = new Queue<int>(),
                [nonStriker] = new Queue<int>()
            };

            string lastBowler = null;

            for (int over = 1; over <= totalOvers; over++)
            {
                var currentOver = new List<string>();
                var runsThisOver = 0;
                var wicketsThisOver = 0;

                var eligibleBowlers = bowlingCandidates
                    .Where(b => bowlerOversCount[b.Name] < maxOversPerBowler && b.Name != lastBowler)
                    .ToList();

                if (eligibleBowlers.Count == 0)
                    eligibleBowlers = bowlingCandidates.Where(b => bowlerOversCount[b.Name] < maxOversPerBowler).ToList();
                if (eligibleBowlers.Count == 0)
                    eligibleBowlers = bowlingCandidates.ToList();

                var currentBowler = eligibleBowlers[_random.Next(eligibleBowlers.Count)];
                lastBowler = currentBowler.Name;
                bowlerOversCount[currentBowler.Name]++;

                var currentBowlerType = currentBowler.BowlingType;

                for (int ball = 1; ball <= SimulationConstants.BallsPerOver; ball++)
                {
                    if (totalWickets >= 10)
                    {
                        currentOver.Add("Innings Ended (All Out)");
                        allOvers.Add(currentOver);
                        overStatsList.Add(new OverStat
                        {
                            OverNumber = over,
                            Bowler = $"{currentBowler.Name} ({currentBowlerType})",
                            Deliveries = [.. currentOver],
                            Runs = runsThisOver,
                            Wickets = wicketsThisOver
                        });
                        goto EndInnings;
                    }
                    if (isSecondInnings && targetScore.HasValue && totalRuns >= targetScore.Value)
                    {
                        currentOver.Add($"Target chased in {over - 1}.{ball - 1} overs!");
                        allOvers.Add(currentOver);
                        overStatsList.Add(new OverStat
                        {
                            OverNumber = over,
                            Bowler = $"{currentBowler.Name} ({currentBowlerType})",
                            Deliveries = [.. currentOver],
                            Runs = runsThisOver,
                            Wickets = wicketsThisOver
                        });
                        goto EndInnings;
                    }

                    int pressure = MatchPressureManager.CalculatePressure(
                        scenario.GameType, over - 1, totalOvers, totalRuns, totalWickets, targetScore);

                    int batterStamina = PlayerStaminaManager.GetBatterStamina(striker);
                    double modifier = PlayerStaminaManager.GetPerformanceModifierFromStamina(batterStamina);
                    int adjustedAggression = (int)(scenario.BattingAggression * modifier);
                    adjustedAggression = MatchSimulationHelper.ApplyBatterTypeModifier(
                        battingTeam.First(b => b.Name == striker).BattingType, adjustedAggression, over, totalOvers);

                    int matchupModifier = 0;
                    var batterType = battingTeam.First(b => b.Name == striker).BattingType;
                    var bowlerTypeCheck = currentBowlerType.ToLower();
                    if (batterType == "Anchor" && bowlerTypeCheck == SimulationConstants.BowlerSpin)
                        matchupModifier += SimulationConstants.AnchorVsSpinPenalty;
                    if (batterType == "Aggressive" && bowlerTypeCheck == "swing")
                        matchupModifier += SimulationConstants.AggressiveVsSwingPenalty;
                    if (batterType == "Finisher" && bowlerTypeCheck == SimulationConstants.BowlerSpin)
                        matchupModifier += SimulationConstants.FinisherVsSpinBoost;
                    adjustedAggression += matchupModifier;

                    int bowlerAggression = scenario.BowlingAggression;
                    if (scenario.Weather.ToLower() == SimulationConstants.WeatherCloudy && bowlerTypeCheck == "swing")
                        bowlerAggression += SimulationConstants.CloudyWeatherSwingBonus;
                    if (scenario.Weather.ToLower() == SimulationConstants.WeatherDry && bowlerTypeCheck == SimulationConstants.BowlerSpin)
                        bowlerAggression += SimulationConstants.DryWeatherSpinBonus;

                    int contextMod = MatchPressureManager.GetContextualAggressionModifier(
                        scenario.GameType, over - 1, totalOvers, totalWickets, totalRuns, targetScore ?? 0);
                    adjustedAggression += contextMod;

                    if (recentRuns.ContainsKey(striker))
                    {
                        var last6 = recentRuns[striker].TakeLast(6).Sum();
                        var last10 = recentRuns[striker].TakeLast(10).Sum();
                        if (last6 >= 10) adjustedAggression += 5;
                        if (last10 <= 5) adjustedAggression -= 5;
                    }
                    adjustedAggression += _random.Next(SimulationConstants.MinRandomAggressionAdjust, SimulationConstants.MaxRandomAggressionAdjust + 1);
                    adjustedAggression = Math.Clamp(adjustedAggression, 1, 100);

                    int bowlerStamina = PlayerStaminaManager.GetBowlerStamina(currentBowler.Name);
                    var outcome = MatchSimulationHelper.SimulateBall(
                        adjustedAggression, bowlerAggression, scenario.GameType,
                        scenario.PitchType, scenario.Weather, currentBowlerType,
                        scenario.CurrentDay, bowlerOversCount[currentBowler.Name], pressure, bowlerStamina);

                    currentOver.Add($"{striker}: {outcome}");
                    BatterStatsManager.RecordBall(striker, outcome);
                    BowlerStatsManager.RecordDelivery(currentBowler.Name, outcome);

                    if (outcome == "W")
                    {
                        totalWickets++;
                        wicketsThisOver++;
                        if (nextBatterIndex < battingTeam.Count)
                        {
                            striker = battingTeam[nextBatterIndex].Name;
                            nextBatterIndex++;
                            recentRuns[striker] = new Queue<int>();
                        }
                        else
                        {
                            currentOver.Add("No batters left. All Out.");
                            allOvers.Add(currentOver);
                            overStatsList.Add(new OverStat
                            {
                                OverNumber = over,
                                Bowler = $"{currentBowler.Name} ({currentBowlerType})",
                                Deliveries = [.. currentOver],
                                Runs = runsThisOver,
                                Wickets = wicketsThisOver
                            });
                            goto EndInnings;
                        }
                    }
                    else
                    {
                        int run = int.Parse(outcome);
                        if (!recentRuns.ContainsKey(striker))
                            recentRuns[striker] = new Queue<int>();
                        recentRuns[striker].Enqueue(run);
                        if (recentRuns[striker].Count > 10)
                            recentRuns[striker].Dequeue();

                        totalRuns += run;
                        runsThisOver += run;

                        PlayerStaminaManager.ReduceBatterStamina(striker, run, pressure);

                        if (run % 2 == 1)
                            (nonStriker, striker) = (striker, nonStriker);
                    }
                }
                (nonStriker, striker) = (striker, nonStriker);
                allOvers.Add(currentOver);
                overStatsList.Add(new OverStat
                {
                    OverNumber = over,
                    Bowler = $"{currentBowler.Name} ({currentBowlerType})",
                    Deliveries = [.. currentOver],
                    Runs = runsThisOver,
                    Wickets = wicketsThisOver
                });

                if (totalWickets >= 10) break;
            }

        EndInnings:
            return new SimulationResult
            {
                Message = $"Simulated {totalOvers} over(s)" + (isSecondInnings && targetScore.HasValue && totalRuns >= targetScore.Value ? " (Target Chased)" : ""),
                Pitch = scenario.PitchType,
                Weather = scenario.Weather,
                Runs = totalRuns,
                Wickets = totalWickets,
                OversDetail = allOvers,
                OverStats = overStatsList,
                BatterStats = BatterStatsManager.GetAllStats(),
                BowlerStats = BowlerStatsManager.GetAllStats(),
                IsChase = isSecondInnings,
                TargetScore = targetScore
            };
        }
    }
}
