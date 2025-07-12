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

            var batters = scenario.Batters;
            int strikerIndex = 0;
            int nonStrikerIndex = 1;
            int nextBatterIndex = 2;

            if (batters.Count < 2)
              throw new Exception("At least 2 batters are required to start the simulation.");
              
            string striker = batters[strikerIndex].Name;
            string nonStriker = batters[nonStrikerIndex].Name;

            BatterStatsManager.Initialize(batters);

            for (int over = 1; over <= totalOvers; over++)
            {
                var currentOver = new List<string>();
                var runsThisOver = 0;
                var wicketsThisOver = 0;

                var bowlerType = (scenario.BowlerTypes.Count > (over - 1))
                    ? scenario.BowlerTypes[over - 1]
                    : scenario.BowlerTypes.Last();

                if (!bowlerSpells.ContainsKey(bowlerType))
                {
                    bowlerSpells[bowlerType] = 0;
                }

                int spellCount = bowlerSpells[bowlerType];
                bowlerSpells[bowlerType]++;

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

                    var outcome = MatchSimulationHelper.SimulateBall(
                        scenario.BattingAggression,
                        scenario.BowlingAggression,
                        scenario.GameType,
                        scenario.PitchType,
                        scenario.Weather,
                        bowlerType,
                        scenario.CurrentDay,
                        spellCount
                    );

                    currentOver.Add($"{striker}: {outcome}");
                    BatterStatsManager.RecordBall(striker, outcome);
                    if (outcome == "W")
                    {
                        totalWickets++;
                        wicketsThisOver++;

                        if (nextBatterIndex < batters.Count)
                        {
                            striker = batters[nextBatterIndex].Name;
                            nextBatterIndex++;
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
                            goto EndInnings;
                        }
                    }
                    else
                    {
                        int run = int.Parse(outcome);
                        totalRuns += run;
                        runsThisOver += run;

                        if (run % 2 == 1)
                        {
                            var temp = striker;
                            striker = nonStriker;
                            nonStriker = temp;
                        }
                    }
                }

                var tempStriker = striker;
                striker = nonStriker;
                nonStriker = tempStriker;

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
                BatterStats = BatterStatsManager.GetAllStats()
            };
        }
    }
}