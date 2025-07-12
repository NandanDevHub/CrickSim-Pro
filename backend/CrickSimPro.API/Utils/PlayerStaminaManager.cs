using CrickSimPro.Constants;

namespace CrickSimPro.Utils;

public static class PlayerStaminaManager
{
    private static Dictionary<string, int> BatterStamina = new();
    private static Dictionary<string, int> BowlerStamina = new();

    public static void InitializeStamina(List<string> batters, List<string> bowlers)
    {
        foreach (var batter in batters)
            BatterStamina[batter] = SimulationConstants.MaxStamina;

        foreach (var bowler in bowlers)
            BowlerStamina[bowler] = SimulationConstants.MaxStamina;
    }

    public static void ReduceBowlerStamina(string bowlerName)
    {
        if (!BowlerStamina.ContainsKey(bowlerName)) return;

        BowlerStamina[bowlerName] -= SimulationConstants.StaminaLossPerOver;
        BowlerStamina[bowlerName] = Math.Max(BowlerStamina[bowlerName], 0);
    }

    public static void ReduceBatterStamina(string batterName, int runsThisBall)
    {
        if (!BatterStamina.ContainsKey(batterName)) return;

        int totalLoss = SimulationConstants.StaminaLossPerBall + (runsThisBall > 0 ? SimulationConstants.StaminaLossPerRun * runsThisBall : 0);
        BatterStamina[batterName] -= totalLoss;
        BatterStamina[batterName] = Math.Max(BatterStamina[batterName], 0);
    }

    public static int GetBowlerStamina(string bowlerName)
    {
        return BowlerStamina.TryGetValue(bowlerName, out var stamina) ? stamina : SimulationConstants.MaxStamina;
    }

    public static int GetBatterStamina(string batterName)
    {
        return BatterStamina.TryGetValue(batterName, out var stamina) ? stamina : SimulationConstants.MaxStamina;
    }

    public static double GetPerformanceModifierFromStamina(int stamina)
    {
        if (stamina >= 70) return 1.0;     // full performance
        if (stamina >= 40) return 0.9;     // slight dip
        if (stamina >= 20) return 0.75;    // major dip
        return 0.6;                         // exhaustion
    }
}
