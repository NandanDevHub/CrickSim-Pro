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
            int pitchModifier = pitchType.ToLower() switch
            {
                SimulationConstants.PitchGreen => -2,
                SimulationConstants.PitchDry => 1,
                SimulationConstants.PitchNormal => 2,
                _ => 0
            };

            int weatherModifier = weather.ToLower() switch
            {
                SimulationConstants.WeatherSunny => 1,
                SimulationConstants.WeatherCloudy => -2,
                SimulationConstants.WeatherHumid => -1,
                _ => 0
            };

            int dayModifier = 0;
            if (gameType.ToUpper() == SimulationConstants.GameTypeTest)
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
            int modifier = 0;

            if (bowlerType.ToLower() == SimulationConstants.BowlerSpin)
            {
                if (pitchType.ToLower() == SimulationConstants.PitchDry) modifier += 2;
                if (pitchType.ToLower() == SimulationConstants.PitchNormal) modifier += 1;
                if (weather.ToLower() == SimulationConstants.WeatherSunny) modifier += 1;

                if (gameType.ToUpper() == SimulationConstants.GameTypeTest && currentDay >= 4) modifier += 2;
            }
            else if (bowlerType.ToLower() == SimulationConstants.BowlerPace)
            {
                if (pitchType.ToLower() == SimulationConstants.PitchGreen) modifier += 2;
                if (weather.ToLower() == SimulationConstants.WeatherCloudy) modifier += 2;
                if (weather.ToLower() == SimulationConstants.WeatherHumid) modifier += 1;

                if (gameType.ToUpper() == SimulationConstants.GameTypeTest && currentDay <= 2) modifier += 1;
            }

            if (spellCount > 3)
            {
                modifier -= (spellCount - 2);
            }

            return modifier;
        }
    }
}
