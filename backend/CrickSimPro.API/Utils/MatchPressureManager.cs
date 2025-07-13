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
}
