using CrickSimPro.Constants;
using CrickSimPro.API.Models;

namespace CrickSimPro.Utils
{
    /// <summary>
    /// Manages stamina/fatigue for batters and bowlers for a single simulation run.
    /// Impacts aggression, performance modifiers, and sim realism.
    /// NOT THREAD SAFE: Always use fresh state (call InitializeStamina) for each match/innings.
    /// </summary>
    public static class PlayerStaminaManager
    {
        // Per-simulation state: stamina for each player
        private static Dictionary<string, int> BatterStamina = new();
        private static Dictionary<string, int> BowlerStamina = new();

        /// <summary>
        /// Initializes all batters and bowlers to full stamina at the start of a match/innings.
        /// Always call before each new simulation or innings.
        /// </summary>
        public static void InitializeStamina(List<BatterProfile> batters, List<string> bowlers)
        {
            // Fresh state, so previous runs do not interfere
            BatterStamina = new Dictionary<string, int>();
            BowlerStamina = new Dictionary<string, int>();

            foreach (var batter in batters ?? Enumerable.Empty<BatterProfile>())
                BatterStamina[batter.Name] = SimulationConstants.MaxStamina;

            foreach (var bowler in bowlers ?? Enumerable.Empty<string>())
                BowlerStamina[bowler] = SimulationConstants.MaxStamina;
        }

        /// <summary>
        /// Reduces bowler stamina by fixed amount per over (simulates fatigue).
        /// </summary>
        public static void ReduceBowlerStamina(string bowlerName)
        {
            if (string.IsNullOrWhiteSpace(bowlerName) || !BowlerStamina.ContainsKey(bowlerName)) return;
            BowlerStamina[bowlerName] -= SimulationConstants.StaminaLossPerOver;
            BowlerStamina[bowlerName] = Math.Max(BowlerStamina[bowlerName], 0);
        }

        /// <summary>
        /// Reduces batter stamina per ball faced, runs run, and pressure (increases fatigue in crunch situations).
        /// </summary>
        public static void ReduceBatterStamina(string batterName, int runsThisBall, int pressure)
        {
            if (string.IsNullOrWhiteSpace(batterName) || !BatterStamina.ContainsKey(batterName)) return;

            int totalLoss = SimulationConstants.StaminaLossPerBall;
            if (runsThisBall > 0)
                totalLoss += SimulationConstants.StaminaLossPerRun * runsThisBall;

            // Pressure: more fatigue in high-pressure moments
            int pressureFatigue = pressure / 30; // Tweak as needed
            totalLoss += pressureFatigue;

            BatterStamina[batterName] -= totalLoss;
            BatterStamina[batterName] = Math.Max(BatterStamina[batterName], 0);
        }

        /// <summary>
        /// Gets the current stamina of a bowler (0-100).
        /// </summary>
        public static int GetBowlerStamina(string bowlerName)
        {
            return !string.IsNullOrWhiteSpace(bowlerName) && BowlerStamina.TryGetValue(bowlerName, out var stamina)
                ? stamina
                : SimulationConstants.MaxStamina;
        }

        /// <summary>
        /// Gets the current stamina of a batter (0-100).
        /// </summary>
        public static int GetBatterStamina(string batterName)
        {
            return !string.IsNullOrWhiteSpace(batterName) && BatterStamina.TryGetValue(batterName, out var stamina)
                ? stamina
                : SimulationConstants.MaxStamina;
        }

        /// <summary>
        /// Returns a performance modifier [0.6, 1.0] based on stamina level.
        /// Lower stamina = steeper performance drop-off.
        /// </summary>
        public static double GetPerformanceModifierFromStamina(int stamina)
        {
            if (stamina >= 70) return 1.0;      // Peak performance
            if (stamina >= 40) return 0.9;      // Mild dip
            if (stamina >= 20) return 0.75;     // Heavy dip
            return 0.6;                         // Extreme fatigue
        }

        /// <summary>
        /// Clears all stamina for all players (for resets/repeats/test mode).
        /// </summary>
        public static void Reset()
        {
            BatterStamina.Clear();
            BowlerStamina.Clear();
        }

        // [Future] Add support for injury, rest/substitution, match-long fatigue accumulation, etc.
    }
}
