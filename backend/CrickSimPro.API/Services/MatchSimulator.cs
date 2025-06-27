using CrickSimPro.API.Models;

namespace CrickSimPro.API.Services;

public class MatchSimulator
{
    private readonly Random _random = new();
    public SimulationResult Simulate(MatchScenario scenario)
    {
        int totalOvers = scenario.Overs > 0 ? scenario.Overs : GetDefaultOvers(scenario.GameType);
        var allOvers = new List<List<string>>();
        var overStatsList = new List<OverStat>(); // List to hold over statistics
        int totalRuns = 0;
        int totalWickets = 0;

        for (int over = 1; over <= totalOvers; over++)
        {
            var currentOver = new List<string>();
            var runsThisOver = 0;
            var wicketsThisOver = 0;

            var bowlerType = (scenario.BowlerTypes.Count > (over - 1)) ? scenario.BowlerTypes[over - 1] : scenario.BowlerTypes.Last();

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

                var outcome = SimulateBall(
                    scenario.BattingAggression,
                    scenario.BowlingAggression,
                    scenario.GameType,
                    scenario.PitchType,
                    scenario.Weather,
                    bowlerType,
                    scenario.CurrentDay
                );

                currentOver.Add(outcome);

                if (outcome == "W")
                {
                    totalWickets++;
                    wicketsThisOver++;
                }
                else
                {
                    int run = int.Parse(outcome);
                    totalRuns += run;
                    runsThisOver += run;
                }
            }
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
            Message = $"Simulated {scenario.Overs} over(s)",
            Pitch = scenario.PitchType,
            Weather = scenario.Weather,
            Runs = totalRuns,
            Wickets = totalWickets,
            OversDetail = allOvers,
            OverStats = overStatsList
        };
    }

    private string SimulateBall(int battingaggression, int bowlingaggression, string gameType, string pitchType, string weather, string bowlerType, int currentDay)
    {
        (int pitchMod, int weatherMod) = GetConditionsImpact(pitchType, weather, gameType, currentDay); int bowlerMod = bowlerType.ToLower() switch
        {
            "pace" => (pitchType.ToLower() == "green" || weather.ToLower() == "cloudy") ? 2 : 0,
            "swing" => (weather.ToLower() == "cloudy" || weather.ToLower() == "humid") ? 2 : 0,
            "spin" => (pitchType.ToLower() == "dry") ? 2 : 0,
            _ => 0
        };

        int effectiveAggression = Math.Clamp(battingaggression - bowlingaggression + 5 + pitchMod + weatherMod - bowlerMod, 1, 10);
        int chance = _random.Next(100);
        gameType = gameType.ToUpper();

        if (gameType == "T20")
        {
            if (chance < 15 - effectiveAggression) return "0";
            if (chance < 50) return "1";
            if (chance < 65) return "2";
            if (chance < 80) return "4";
            if (chance < 90 + effectiveAggression / 2) return "6";
            return "W";
        }
        else if (gameType == "ODI")
        {
            if (chance < 25 - effectiveAggression) return "0";
            if (chance < 55) return "1";
            if (chance < 70) return "2";
            if (chance < 82) return "4";
            if (chance < 88 + effectiveAggression / 2) return "6";
            return "W";
        }
        else if (gameType == "TEST")
        {
            if (chance < 35 - effectiveAggression) return "0";
            if (chance < 60) return "1";
            if (chance < 75) return "2";
            if (chance < 85) return "4";
            if (chance < 90 + effectiveAggression / 2) return "6";
            return "W";
        }
        else
        {
            if (chance < 30 - effectiveAggression) return "0";
            if (chance < 60) return "1";
            if (chance < 75) return "2";
            if (chance < 85) return "4";
            if (chance < 90 + effectiveAggression / 2) return "6";
            return "W";
        }
    }

    private int GetDefaultOvers(string gameType)
    {
        return gameType.ToUpper() switch
        {
            "Test" => 90,
            "ODI" => 50,
            "T20" => 20,
            _ => 20
        };
    }

    private (int pitchModifier, int weatherModifier) GetConditionsImpact(string pitchType, string weather, string gameType, int currentDay)
    {
        int pitchModifier = pitchType.ToLower() switch
        {
            "green" => -2,
            "dry" => 1,
            "normal" => 2,
            _ => 0
        };

        int weatherModifier = weather.ToLower() switch
        {
            "sunny" => 1,
            "cloudy" => -2,
            "humid" => -1,
            _ => 0
        };

        int dayModifier = 0;

        if (gameType.ToUpper() == "TEST")
        {
            dayModifier = currentDay switch
            {
                1 => -1,
                2 => 0,
                3 => 1,
                4 => 2,
                5 => 3,
                _ => 0
            };
        }

        return (pitchModifier + dayModifier, weatherModifier);
    }
}
