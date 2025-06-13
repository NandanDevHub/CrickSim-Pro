using CrickSimPro.API.Models;

namespace CrickSimPro.API.Services;
public class MatchSimulator
{
    private readonly Random _random = new();
    public object Simulate(MatchScenario scenario)
    {

        var allBalls = new List<string>(); 
        int totalRuns = 0;
        int totalWickets = 0;

        for (int over = 0; over <= scenario.Overs; over++)
        {
            for (int ball = 1; ball <= 6; ball++)
            {
                if (totalWickets >= 10)
                {
                    allBalls.Add("Innings Ended (All Out)");
                    goto EndOfInnings;
                }

                var outcome = SimulateBall(scenario.BattingAggression, scenario.BowlingAggression);
                allBalls.Add(outcome);

                if (outcome == "W")
                    totalWickets++;
                else
                    totalRuns += int.Parse(outcome);
            }
        }
        
EndOfInnings:
        return new SimulationResult
        {
            Message = $"Simulated {scenario.Overs} Overs",
            Pitch = scenario.PitchType,
            Weather = scenario.Weather,
            Runs = totalRuns,
            Wickets = totalWickets,
            BallByBall = allBalls
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
