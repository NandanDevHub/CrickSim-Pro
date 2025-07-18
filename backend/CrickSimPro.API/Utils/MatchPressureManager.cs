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

        // Base pressure from wickets lost
        pressure += wicketsLost * SimulationConstants.PressurePerWicket;

        gameType = gameType?.ToUpperInvariant() ?? "";

        // Apply only for ODIs and T20s, and only if target present (chase)
        if ((gameType == SimulationConstants.GameTypeODI || gameType == SimulationConstants.GameTypeT20) && targetScore.HasValue)
        {
            int runsRemaining = targetScore.Value - runsScored;
            int oversRemaining = totalOvers - oversCompleted;

            if (oversRemaining > 0)
            {
                double requiredRunRate = (double)runsRemaining / oversRemaining;
                double currentRunRate = oversCompleted > 0 ? (double)runsScored / oversCompleted : 0;

                if (requiredRunRate > currentRunRate)
                {
                    pressure += (requiredRunRate - currentRunRate) * SimulationConstants.PressurePerRRGap;
                }
            }
        }

        // Clamp pressure to [0, 100]
        pressure = Math.Max(0, Math.Min(pressure, 100));
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
        int modifier = 0;
        gameType = gameType?.ToUpperInvariant() ?? "";

        // Only for ODI/T20 chases
        if ((gameType == SimulationConstants.GameTypeODI || gameType == SimulationConstants.GameTypeT20) && targetScore > 0)
        {
            double requiredRunRate = (oversLeft > 0)
                ? (double)(targetScore - currentRuns) / oversLeft
                : 0;

            // Boost aggression if required run rate is very high in last overs
            if (requiredRunRate >= 8 && oversLeft <= 5)
                modifier += 10;
            else if (requiredRunRate >= 6 && oversLeft <= 10)
                modifier += 5;
        }

        // Lower aggression if many wickets lost
        if (wicketsLost >= 7) modifier -= 10;
        else if (wicketsLost >= 5) modifier -= 5;

        return Math.Clamp(modifier, -10, 15);
    }
}
