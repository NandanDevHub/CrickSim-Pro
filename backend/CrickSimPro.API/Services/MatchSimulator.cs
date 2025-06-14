using CrickSimPro.API.Models;

namespace CrickSimPro.API.Services;

public class MatchSimulator
{
    private readonly Random _random = new();
    public SimulationResult Simulate(MatchScenario scenario)
    {
        var allOvers = new List<List<string>>();
        int totalRuns = 0;
        int totalWickets = 0;

        for (int over = 1; over <= scenario.Overs; over++)
        {
            var currentOver = new List<string>();

            for (int ball = 1; ball <= 6; ball++)
            {
                if (totalWickets >= 10)
                {
                    currentOver.Add("Innings Ended (All Out)");
                    allOvers.Add(currentOver);
                    goto EndInnings;
                }

                var outcome = SimulateBall(scenario.BattingAggression, scenario.BowlingAggression);
                currentOver.Add(outcome);

                if (outcome == "W") totalWickets++;
                else totalRuns += int.Parse(outcome);
            }

            allOvers.Add(currentOver);
        }

    EndInnings:
        return new SimulationResult
        {
            Message = $"Simulated {scenario.Overs} over(s)",
            Pitch = scenario.PitchType,
            Weather = scenario.Weather,
            Runs = totalRuns,
            Wickets = totalWickets,
            OversDetail = allOvers
        };
    }

    private string SimulateBall(int battingaggression, int bowlingaggression)
    {
        int baseaggression = battingaggression + bowlingaggression;
        int effectiveAggression = Math.Clamp(baseaggression + 5, 1, 10);

        int chance = _random.Next(100);

        if (chance < 10 - effectiveAggression) return "0";
        if (chance < 30) return "1";
        if (chance < 50) return "2";
        if (chance < 65) return "4";
        if (chance < 75) return "6";
        return "W";
    }
}
