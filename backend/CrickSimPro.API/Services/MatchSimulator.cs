using CrickSimPro.API.Models;

namespace CrickSimPro.API.Services;
public class MatchSimulator
{
    private readonly Random _random = new();
    public object Simulate(MatchScenario scenario)
    {
        var outcomes = new List<string>();
        int runs = 0;
        int wickets = 0;

        for (int i = 0; i < 6; i++)
        {
            var outcome = SimulateBall(scenario.BattingAggression, scenario.BowlingAggression);
            outcomes.Add(outcome);

            if (outcome == "W")
                wickets++;
            else
                runs += int.Parse(outcome);
        }

        return new SimulationResult
        {
            Message = "Over Simulatted",
            Pitch = scenario.PitchType,
            Weather = scenario.Weather,
            Runs = runs,
            Wickets = wickets,
            BallByBall = outcomes
        };
    }

    private string SimulateBall(int battingaggression, int bowlingaggression)
    {
        int baseaggression = battingaggression + bowlingaggression;
        int effectiveAggression = Math.Clamp(baseaggression + 5,1, 10);

        int chance = _random.Next(100);

        if (chance < 10 - effectiveAggression) return "0";
        if (chance < 30) return "1";
        if (chance < 50) return "2";
        if (chance < 65) return "4";
        if (chance < 75) return "6";
        return "W";
    }
}
