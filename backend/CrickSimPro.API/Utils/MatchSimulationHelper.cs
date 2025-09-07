using CrickSimPro.Constants;
using System;
using System.Collections.Generic;

namespace CrickSimPro.Utils
{
    public static class MatchSimulationHelper
    {
        private static readonly Random _random = new();
        public static (string outcome, string? howOut) SimulateBallWithWicketMode(
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
            int extraRoll = _random.Next(100);
            if (extraRoll < 5) return ("WD", null);
            if (extraRoll < 10) return ("NB", null);
            if (extraRoll < 13) return ("B", null);
            if (extraRoll < 16) return ("LB", null);

            string outcome = SimulateBall(
                battingAggression, bowlingAggression, gameType, pitchType, weather, bowlerType,
                currentDay, spellCount, pressure, bowlerStamina);

            if (outcome != "W")
                return (outcome, null);

            var modes = new List<(string mode, int weight)>
            {
                ("Bowled", 20),
                ("Caught", 30),
                ("LBW", 15),
                ("Run Out", 10),
                ("Stumped", bowlerType == SimulationConstants.BowlerSpin ? 15 : 5),
                ("Hit Wicket", 2),
                ("Caught & Bowled", 8),
                ("Timed Out", 1),
                ("Handled Ball", 1),
                ("Obstructing the Field", 1)
            };

            var lbw = modes.Find(m => m.mode == "LBW");
            if (lbw.mode != null && weather.ToLower() == SimulationConstants.WeatherWet && bowlerType == SimulationConstants.BowlerSwing)
            {
                int idx = modes.FindIndex(m => m.mode == "LBW");
                modes[idx] = (lbw.mode, lbw.weight + 5);
            }
            var runOut = modes.Find(m => m.mode == "Run Out");
            if (runOut.mode != null && pressure > 50)
            {
                int idx = modes.FindIndex(m => m.mode == "Run Out");
                modes[idx] = (runOut.mode, runOut.weight + 5);
            }

            int total = 0;
            foreach (var (mode, weight) in modes) total += weight;
            int pick = _random.Next(total);
            int cumulative = 0;
            foreach (var (mode, weight) in modes)
            {
                cumulative += weight;
                if (pick < cumulative)
                    return ("W", mode);
            }

            // Adding a fallback
            return ("W", "Caught");
        }

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
            gameType = gameType?.ToUpperInvariant() ?? "";
            pitchType = pitchType?.ToLowerInvariant() ?? "";
            weather = weather?.ToLowerInvariant() ?? "";
            bowlerType = bowlerType?.ToLowerInvariant() ?? "";

            (int pitchMod, int weatherMod) = MatchConditions.GetConditionsImpact(
                pitchType, weather, gameType, currentDay);

            int bowlerMod = MatchConditions.GetBowlerEffectiveness(
                bowlerType, pitchType, weather, gameType, currentDay, spellCount);

            int effectiveAggression = Math.Clamp(
                battingAggression - bowlingAggression + 5 + pitchMod + weatherMod - bowlerMod,
                1, 100);

            if (bowlerStamina < 25) effectiveAggression += 4;
            else if (bowlerStamina < 40) effectiveAggression += 2;

            int pressureImpact = Math.Clamp(pressure / 7, 0, 15);
            int chance = Math.Max(0, _random.Next(100) + pressureImpact);

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

                _ =>
                    chance < 30 - (effectiveAggression / 10) ? "0" :
                    chance < 60 ? "1" :
                    chance < 75 ? "2" :
                    chance < 85 ? "4" :
                    chance < 90 + effectiveAggression / 12 ? "6" : "W",
            };
        }

        public static int GetDefaultOvers(string gameType)
        {
            gameType = gameType?.ToUpperInvariant() ?? "";
            return gameType switch
            {
                SimulationConstants.GameTypeTest => SimulationConstants.TestOvers,
                SimulationConstants.GameTypeODI => SimulationConstants.ODIOvers,
                SimulationConstants.GameTypeT20 => SimulationConstants.T20Overs,
                _ => SimulationConstants.T20Overs
            };
        }

        public static int ApplyBatterTypeModifier(string batterType, int aggression, int currentOver, int totalOvers)
        {
            batterType = batterType?.Trim() ?? "";
            return batterType switch
            {
                "Aggressive" => Math.Min(100, aggression + 15),
                "Anchor" => currentOver < 10 ? Math.Max(1, aggression - 10) : aggression,
                "Finisher" => currentOver >= totalOvers - 5 ? Math.Min(100, aggression + 20) : aggression,
                "Tailender" => Math.Max(1, aggression - 15),
                _ => aggression
            };
        }
    }
}
