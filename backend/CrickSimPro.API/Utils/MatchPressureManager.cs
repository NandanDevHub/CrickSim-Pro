using CrickSimPro.Constants;

namespace CrickSimPro.Utils;

public static class MatchPressureManager
{
    public static int CalculatePressure(
        string gameType,
        int oversCompleted,
        int totalOvers,
        int runsScored,
        int wicketsLost,
        int? targetScore = null
    )
    {
        double pressure = 0;

        // Adding the Base pressure from wickets
        pressure += wicketsLost * SimulationConstants.PressurePerWicket;

        // If chasing a target
        if (targetScore.HasValue)
        {
            int runsRemaining = targetScore.Value - runsScored;
            int oversRemaining = totalOvers - oversCompleted;

            if (oversRemaining > 0)
            {
                double requiredRunRate = (double)runsRemaining / oversRemaining;
                double currentRunRate = (double)runsScored / (oversCompleted == 0 ? 1 : oversCompleted);

                if (requiredRunRate > currentRunRate)
                {
                    pressure += (requiredRunRate - currentRunRate) * SimulationConstants.PressurePerRRGap;
                }
            }
        }

        pressure = Math.Min(pressure, 100);
        pressure = Math.Max(pressure, 0);

        return (int)Math.Round(pressure);
    }
    public static int GetContextualAggressionModifier(
        string gameType,
        int oversCompleted,
        int totalOvers,
        int wicketsLost,
        int currentRuns,
        int targetScore)
    {
        int oversLeft = totalOvers - oversCompleted;
        int requiredRunRate = (targetScore > 0 && oversLeft > 0)
            ? (int)Math.Ceiling((double)(targetScore - currentRuns) / oversLeft)
            : 0;

        int modifier = 0;

        // Increasing aggression if chasing high and few overs left
        if (targetScore > 0)
        {
            if (requiredRunRate >= 8 && oversLeft <= 5)
                modifier += 10;
            else if (requiredRunRate >= 6 && oversLeft <= 10)
                modifier += 5;
        }

        // Lower down the aggression if many wickets lost
        if (wicketsLost >= 7) modifier -= 10;
        else if (wicketsLost >= 5) modifier -= 5;

        return Math.Clamp(modifier, -10, 15);
    }

}
