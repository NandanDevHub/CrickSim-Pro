using CrickSimPro.Constants;

namespace CrickSimPro.Utils
{
    public static class MatchConditions
    {
        public static (int pitchModifier, int weatherModifier) GetConditionsImpact(
            string pitchType,
            string weather,
            string gameType,
            int currentDay)
        {
            pitchType = pitchType?.ToLowerInvariant() ?? "";
            weather = weather?.ToLowerInvariant() ?? "";
            gameType = gameType?.ToUpperInvariant() ?? "";

            // --- Pitch impact ---
            int pitchModifier = pitchType switch
            {
                SimulationConstants.PitchGreen => -2,  // adding more movement for seamers
                SimulationConstants.PitchDry => 1,     // to help spinners
                SimulationConstants.PitchNormal => 2,  // batter-friendly
                _ => 0
            };

            // --- Weather impact ---
            int weatherModifier = weather switch
            {
                SimulationConstants.WeatherSunny => 1,     // easier for batters
                SimulationConstants.WeatherCloudy => -2,   // helps swing/seam
                SimulationConstants.WeatherHumid => -1,    // helps swing
                _ => 0
            };

            // --- Test match day-wise effect ---
            // Only applies in Test matches (pitch wears out, helps bowlers later)
            int dayModifier = 0;
            if (gameType == SimulationConstants.GameTypeTest)
            {
                dayModifier = currentDay switch
                {
                    1 => -1,  // Fresh pitch, helps bowlers
                    2 => 0,
                    3 => 1,   // Pitch breaking up
                    4 => 2,
                    5 => 3,   // Very worn, helps spinners
                    _ => 0
                };
            }

            // Return combined effect for pitch+day, and weather separately
            return (pitchModifier + dayModifier, weatherModifier);
        }

        public static int GetBowlerEffectiveness(
            string bowlerType,
            string pitchType,
            string weather,
            string gameType,
            int currentDay,
            int spellCount)
        {
            bowlerType = bowlerType?.ToLowerInvariant() ?? "";
            pitchType = pitchType?.ToLowerInvariant() ?? "";
            weather = weather?.ToLowerInvariant() ?? "";
            gameType = gameType?.ToUpperInvariant() ?? "";

            int modifier = 0;

            if (bowlerType == SimulationConstants.BowlerSpin)
            {
                if (pitchType == SimulationConstants.PitchDry) modifier += 2;         // Spinners love dry
                if (pitchType == SimulationConstants.PitchNormal) modifier += 1;
                if (weather == SimulationConstants.WeatherSunny) modifier += 1;       // Ball grips more
                if (gameType == SimulationConstants.GameTypeTest && currentDay >= 4) modifier += 2; // Wear helps spin
            }
            else if (bowlerType == SimulationConstants.BowlerPace)
            {
                if (pitchType == SimulationConstants.PitchGreen) modifier += 2;       // Seamers' dream
                if (weather == SimulationConstants.WeatherCloudy) modifier += 2;      // Swing conditions
                if (weather == SimulationConstants.WeatherHumid) modifier += 1;       // Swing conditions
                if (gameType == SimulationConstants.GameTypeTest && currentDay <= 2) modifier += 1; // New pitch
            }
            // For swing, etc., extend as needed.

            // --- Spell fatigue: If spellCount > 3, bowler gets tired, loses effectiveness ---
            if (spellCount > 3)
                modifier -= (spellCount - 2); // Each extra over beyond 3 reduces effectiveness

            // --- [Future]: Add cases for night games, rain/wet pitch, day/night, etc. ---

            return modifier;
        }
    }
}
