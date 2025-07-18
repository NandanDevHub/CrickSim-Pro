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

            int pitchModifier = pitchType switch
            {
                SimulationConstants.PitchGreen => -2,
                SimulationConstants.PitchDry => 1,
                SimulationConstants.PitchNormal => 2,
                _ => 0
            };

            int weatherModifier = weather switch
            {
                SimulationConstants.WeatherSunny => 1,
                SimulationConstants.WeatherCloudy => -2,
                SimulationConstants.WeatherHumid => -1,
                _ => 0
            };

            int dayModifier = 0;
            if (gameType == SimulationConstants.GameTypeTest)
            {
                dayModifier = currentDay switch
                {
                    1 => -1,
                    2 => 0,
                    3 => 1,
                    4 => 2,
                    5 => 3,
                    _ => 0
                };
            }

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
                if (pitchType == SimulationConstants.PitchDry) modifier += 2;
                if (pitchType == SimulationConstants.PitchNormal) modifier += 1;
                if (weather == SimulationConstants.WeatherSunny) modifier += 1;
                if (gameType == SimulationConstants.GameTypeTest && currentDay >= 4) modifier += 2;
            }
            else if (bowlerType == SimulationConstants.BowlerPace)
            {
                if (pitchType == SimulationConstants.PitchGreen) modifier += 2;
                if (weather == SimulationConstants.WeatherCloudy) modifier += 2;
                if (weather == SimulationConstants.WeatherHumid) modifier += 1;
                if (gameType == SimulationConstants.GameTypeTest && currentDay <= 2) modifier += 1;
            }

            // Bowler spell fatigue effect: negative for longer spells
            if (spellCount > 3)
                modifier -= (spellCount - 2);

            return modifier;
        }
    }
}
