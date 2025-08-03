using CrickSimPro.Constants;

namespace CrickSimPro.Utils
{
    /// <summary>
    /// Contains all ball-by-ball simulation and core calculation helpers for the match engine.
    /// Any core logic change here will directly impact match realism and output.
    /// </summary>
    public static class MatchSimulationHelper
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Simulates a single ball outcome based on current batter, bowler, pitch, and contextual match factors.
        /// Returns: "0", "1", "2", "4", "6", or "W" (wicket).
        /// </summary>
        public static string SimulateBall(
            int battingAggression,
            int bowlingAggression,
            string gameType,
            string pitchType,
            string weather,
            string bowlerType,
            int currentDay,
            int spellCount,
            int pressure,
            int bowlerStamina)
        {
            // --- Normalize all inputs ---
            gameType = gameType?.ToUpperInvariant() ?? "";
            pitchType = pitchType?.ToLowerInvariant() ?? "";
            weather = weather?.ToLowerInvariant() ?? "";
            bowlerType = bowlerType?.ToLowerInvariant() ?? "";

            // --- Get pitch & weather impacts ---
            (int pitchMod, int weatherMod) = MatchConditions.GetConditionsImpact(
                pitchType, weather, gameType, currentDay);

            // --- Get bowler effectiveness (spells, matchups, etc.) ---
            int bowlerMod = MatchConditions.GetBowlerEffectiveness(
                bowlerType, pitchType, weather, gameType, currentDay, spellCount);

            // --- Calculate "effective" aggression for batter this ball ---
            int effectiveAggression = Math.Clamp(
                battingAggression - bowlingAggression + 5 + pitchMod + weatherMod - bowlerMod,
                1, 100);

            // --- Fatigue: If bowler is tired, batter gets a bonus ---
            if (bowlerStamina < 25) effectiveAggression += 4;
            else if (bowlerStamina < 40) effectiveAggression += 2;

            // --- Pressure makes wickets likelier, scoring harder ---
            int pressureImpact = Math.Clamp(pressure / 7, 0, 15);

            // --- RANDOM OUTCOME: The core "chance" roll ---
            int chance = Math.Max(0, _random.Next(100) + pressureImpact);

            // --- Main simulation outcome logic ---
            // For each game type, tune the bands for realism. Future: Extract to config for live tuning.
            // (Extras can be slotted here in future for "WIDE", "NO BALL", etc.)
            return gameType switch
            {
                SimulationConstants.GameTypeT20 =>
                    chance < 15 - (effectiveAggression / 10) ? "0" :
                    chance < 50 ? "1" :
                    chance < 65 ? "2" :
                    chance < 80 ? "4" :
                    chance < 90 + effectiveAggression / 12 ? "6" : "W",

                SimulationConstants.GameTypeODI =>
                    chance < 25 - (effectiveAggression / 12) ? "0" :
                    chance < 55 ? "1" :
                    chance < 70 ? "2" :
                    chance < 82 ? "4" :
                    chance < 88 + effectiveAggression / 12 ? "6" : "W",

                SimulationConstants.GameTypeTest =>
                    chance < 35 - (effectiveAggression / 15) ? "0" :
                    chance < 60 ? "1" :
                    chance < 75 ? "2" :
                    chance < 85 ? "4" :
                    chance < 90 + effectiveAggression / 12 ? "6" : "W",

                // If unknown game type, default to T20 logic for safety, but flag for maintainers
                _ =>
                    // TODO: [CrickSimPro] Unknown game type; review logic if you add new types!
                    chance < 30 - (effectiveAggression / 10) ? "0" :
                    chance < 60 ? "1" :
                    chance < 75 ? "2" :
                    chance < 85 ? "4" :
                    chance < 90 + effectiveAggression / 12 ? "6" : "W",
            };
        }

        /// <summary>
        /// Returns the default number of overs for a given game type.
        /// </summary>
        public static int GetDefaultOvers(string gameType)
        {
            gameType = gameType?.ToUpperInvariant() ?? "";
            return gameType switch
            {
                SimulationConstants.GameTypeTest => SimulationConstants.TestOvers,
                SimulationConstants.GameTypeODI  => SimulationConstants.ODIOvers,
                SimulationConstants.GameTypeT20  => SimulationConstants.T20Overs,
                _ => SimulationConstants.T20Overs // Fallback
            };
        }

        /// <summary>
        /// Applies batter type and match phase to aggression (e.g. openers/anchors start slow, finishers get a boost at end).
        /// </summary>
        public static int ApplyBatterTypeModifier(string batterType, int aggression, int currentOver, int totalOvers)
        {
            batterType = batterType?.Trim() ?? "";
            return batterType switch
            {
                "Aggressive" => Math.Min(100, aggression + 15),
                "Anchor"     => currentOver < 10 ? Math.Max(1, aggression - 10) : aggression,
                "Finisher"   => currentOver >= totalOvers - 5 ? Math.Min(100, aggression + 20) : aggression,
                "Tailender"  => Math.Max(1, aggression - 15),
                _            => aggression // AllRounder or unknown
            };
        }

        // --- [For future] Simulate extras (wide, no-ball), rain (DLS), super over, etc. ---
        // public static string SimulateExtra(...) { ... }
    }
}
