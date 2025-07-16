using CrickSimPro.API.Models;
using CrickSimPro.Utils;

namespace CrickSimPro.API.Services
{
    public class MatchSimulator
    {
        private readonly Random _random = new();

        public SimulationResult Simulate(MatchScenario scenario)
        {
            int totalOvers = scenario.Overs > 0
                ? scenario.Overs
                : MatchSimulationHelper.GetDefaultOvers(scenario.GameType);

            var allOvers = new List<List<string>>();
            var overStatsList = new List<OverStat>();
            int totalRuns = 0;
            int totalWickets = 0;
            var bowlerSpells = new Dictionary<string, int>();
            var bowlerConfidence = new Dictionary<string, int>();
            foreach (var bowler in scenario.BowlerTypes)
                bowlerConfidence[bowler] = 50;
            BowlingRotationManager.Initialize(scenario.BowlerTypes);
            BowlerStatsManager.Initialize(scenario.BowlerTypes);

            var batters = scenario.Batters;
            int strikerIndex = 0;
            int nonStrikerIndex = 1;
            int nextBatterIndex = 2;

            if (batters.Count < 2)
                throw new Exception("At least 2 batters are required to start the simulation.");

            string striker = batters[strikerIndex].Name;
            string nonStriker = batters[nonStrikerIndex].Name;

            var recentRuns = new Dictionary<string, Queue<int>>();
            recentRuns[striker] = new Queue<int>();
            recentRuns[nonStriker] = new Queue<int>();

            PlayerStaminaManager.InitializeStamina(batters, scenario.BowlerTypes);
            BatterStatsManager.Initialize(batters);

            for (int over = 1; over <= totalOvers; over++)
            {
                var currentOver = new List<string>();
                var runsThisOver = 0;
                var wicketsThisOver = 0;

                var bowlerType = BowlingRotationManager.GetNextBowler(
                    scenario.BowlerTypes,
                    over,
                    totalOvers
                );


                PlayerStaminaManager.ReduceBowlerStamina(bowlerType);

                if (!bowlerSpells.TryGetValue(bowlerType, out int spellCount))
                {
                    spellCount = 0;
                    bowlerSpells[bowlerType] = spellCount;
                }

                bowlerSpells[bowlerType] = ++spellCount;

                for (int ball = 1; ball <= 6; ball++)
                {
                    if (totalWickets >= 10)
                    {
                        currentOver.Add("Innings Ended (All Out)");
                        allOvers.Add(currentOver);
                        overStatsList.Add(new OverStat
                        {
                            OverNumber = over,
                            Bowler = bowlerType,
                            Deliveries = [.. currentOver],
                            Runs = runsThisOver,
                            Wickets = wicketsThisOver
                        });

                        goto EndInnings;
                    }
                    int pressure = MatchPressureManager.CalculatePressure(
                        gameType: scenario.GameType,
                        oversCompleted: over - 1,
                        totalOvers: totalOvers,
                        runsScored: totalRuns,
                        wicketsLost: totalWickets,
                        targetScore: scenario.TargetScore
                    );

                    int batterStamina = PlayerStaminaManager.GetBatterStamina(striker);
                    double modifier = PlayerStaminaManager.GetPerformanceModifierFromStamina(batterStamina);
                    int adjustedAggression = (int)(scenario.BattingAggression * modifier);
                    adjustedAggression = MatchSimulationHelper.ApplyBatterTypeModifier(
                        batters.First(b => b.Name == striker).Type,
                        adjustedAggression,
                        over,
                        totalOvers
                    );

                    adjustedAggression -= (pressure / 10);

                        int contextMod = MatchPressureManager.GetContextualAggressionModifier(
                            scenario.GameType,
                            over - 1,
                            totalOvers,
                            totalWickets,
                            totalRuns,
                            scenario.TargetScore ?? 0
                        );
                    adjustedAggression += contextMod;
                    // Momentum modifier
                    if (recentRuns.ContainsKey(striker))
                    {
                        var last6 = recentRuns[striker].TakeLast(6).Sum();
                        var last10 = recentRuns[striker].TakeLast(10).Sum();

                        if (last6 >= 10) adjustedAggression += 5;
                        if (last10 <= 5) adjustedAggression -= 5;
                    }

                    adjustedAggression += _random.Next(-5, 6);
                    adjustedAggression += contextMod; 

                    adjustedAggression = Math.Clamp(adjustedAggression, 1, 100);

                    int bowlerAggression = scenario.BowlingAggression;
                    if (bowlerConfidence.TryGetValue(bowlerType, out int confidence))
                    {
                        if (confidence >= 70) bowlerAggression += 5;
                        else if (confidence <= 30) bowlerAggression -= 5;
                    }

                    int bowlerStamina = PlayerStaminaManager.GetBowlerStamina(bowlerType);
                    var outcome = MatchSimulationHelper.SimulateBall(
                        adjustedAggression,
                        scenario.BowlingAggression,
                        scenario.GameType,
                        scenario.PitchType,
                        scenario.Weather,
                        bowlerType,
                        scenario.CurrentDay,
                        spellCount,
                        pressure,
                        bowlerStamina
                    );

                    currentOver.Add($"{striker}: {outcome}");
                    BatterStatsManager.RecordBall(striker, outcome);
                    BowlerStatsManager.RecordDelivery(bowlerType, outcome);
                    if (outcome == "W")
                    {
                        totalWickets++;
                        wicketsThisOver++;

                        if (nextBatterIndex < batters.Count)
                        {
                            striker = batters[nextBatterIndex].Name;
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
                                Bowler = bowlerType,
                                Deliveries = [.. currentOver],
                                Runs = runsThisOver,
                                Wickets = wicketsThisOver
                            });

                            if (runsThisOver <= 4)
                                bowlerConfidence[bowlerType] = Math.Min(100, bowlerConfidence[bowlerType] + 5);
                            else if (runsThisOver >= 12)
                                bowlerConfidence[bowlerType] = Math.Max(0, bowlerConfidence[bowlerType] - 5);
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
                        {
                            (nonStriker, striker) = (striker, nonStriker);
                        }
                    }
                }

                (nonStriker, striker) = (striker, nonStriker);
                allOvers.Add(currentOver);
                overStatsList.Add(new OverStat
                {
                    OverNumber = over,
                    Bowler = bowlerType,
                    Deliveries = [.. currentOver],
                    Runs = runsThisOver,
                    Wickets = wicketsThisOver
                });

                if (totalWickets >= 10) break;
            }

        EndInnings:
            return new SimulationResult
            {
                Message = $"Simulated {totalOvers} over(s)",
                Pitch = scenario.PitchType,
                Weather = scenario.Weather,
                Runs = totalRuns,
                Wickets = totalWickets,
                OversDetail = allOvers,
                OverStats = overStatsList,
                BatterStats = BatterStatsManager.GetAllStats(),
                BowlerStats = BowlerStatsManager.GetAllStats()
            };
        }
    }
}