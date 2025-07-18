using CrickSimPro.Constants;
using CrickSimPro.API.Models;

namespace CrickSimPro.Utils;

public static class PlayerStaminaManager
{
    private static Dictionary<string, int> BatterStamina = new();
    private static Dictionary<string, int> BowlerStamina = new();

    public static void InitializeStamina(List<BatterProfile> batters, List<string> bowlers)
    {
        // Ensure truly clean state for repeat runs or multi-innings
        BatterStamina = new Dictionary<string, int>();
        BowlerStamina = new Dictionary<string, int>();
        foreach (var batter in batters ?? Enumerable.Empty<BatterProfile>())
            BatterStamina[batter.Name] = SimulationConstants.MaxStamina;

        foreach (var bowler in bowlers ?? Enumerable.Empty<string>())
            BowlerStamina[bowler] = SimulationConstants.MaxStamina;
    }

    public static void ReduceBowlerStamina(string bowlerName)
    {
        if (string.IsNullOrWhiteSpace(bowlerName) || !BowlerStamina.ContainsKey(bowlerName)) return;
        BowlerStamina[bowlerName] -= SimulationConstants.StaminaLossPerOver;
        BowlerStamina[bowlerName] = Math.Max(BowlerStamina[bowlerName], 0);
    }

    public static void ReduceBatterStamina(string batterName, int runsThisBall, int pressure)
    {
        if (string.IsNullOrWhiteSpace(batterName) || !BatterStamina.ContainsKey(batterName)) return;

        int totalLoss = SimulationConstants.StaminaLossPerBall;
        if (runsThisBall > 0)
            totalLoss += SimulationConstants.StaminaLossPerRun * runsThisBall;

        // Pressure = more fatigue in crunch situations
        int pressureFatigue = pressure / 30;
        totalLoss += pressureFatigue;

        BatterStamina[batterName] -= totalLoss;
        BatterStamina[batterName] = Math.Max(BatterStamina[batterName], 0);
    }

    public static int GetBowlerStamina(string bowlerName)
    {
        return !string.IsNullOrWhiteSpace(bowlerName) && BowlerStamina.TryGetValue(bowlerName, out var stamina)
            ? stamina
            : SimulationConstants.MaxStamina;
    }

    public static int GetBatterStamina(string batterName)
    {
        return !string.IsNullOrWhiteSpace(batterName) && BatterStamina.TryGetValue(batterName, out var stamina)
            ? stamina
            : SimulationConstants.MaxStamina;
    }

    public static double GetPerformanceModifierFromStamina(int stamina)
    {
        // Pure realism: fine-tuned drop-offs
        if (stamina >= 70) return 1.0;      // top performance
        if (stamina >= 40) return 0.9;      // mild dip
        if (stamina >= 20) return 0.75;     // heavy dip
        return 0.6;                         // extreme fatigue
    }

    // Extra: Reset if you want to support test mode or hard restarts
    public static void Reset()
    {
        BatterStamina.Clear();
        BowlerStamina.Clear();
    }
}
