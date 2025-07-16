using CrickSimPro.Constants;

namespace CrickSimPro.Utils
{
    public static class MatchSimulationHelper
    {
        private static readonly Random _random = new();

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
            (int pitchMod, int weatherMod) = MatchConditions.GetConditionsImpact(pitchType, weather, gameType, currentDay);
            int bowlerMod = MatchConditions.GetBowlerEffectiveness(bowlerType, pitchType, weather, gameType, currentDay, spellCount);

            int effectiveAggression = Math.Clamp(
                battingAggression - bowlingAggression + 5 + pitchMod + weatherMod - bowlerMod,
                1, 10);

            if (bowlerStamina < 40) effectiveAggression += 1;
            if (bowlerStamina < 25) effectiveAggression += 2;

            int pressureImpact = pressure / 6;
            int chance = _random.Next(100) + pressureImpact;
            gameType = gameType.ToUpper();

            return gameType switch
            {
                SimulationConstants.GameTypeT20 => chance < 15 - effectiveAggression ? "0" :
                                                  chance < 50 ? "1" :
                                                  chance < 65 ? "2" :
                                                  chance < 80 ? "4" :
                                                  chance < 90 + effectiveAggression / 2 ? "6" : "W",

                SimulationConstants.GameTypeODI => chance < 25 - effectiveAggression ? "0" :
                                                  chance < 55 ? "1" :
                                                  chance < 70 ? "2" :
                                                  chance < 82 ? "4" :
                                                  chance < 88 + effectiveAggression / 2 ? "6" : "W",

                SimulationConstants.GameTypeTest => chance < 35 - effectiveAggression ? "0" :
                                                   chance < 60 ? "1" :
                                                   chance < 75 ? "2" :
                                                   chance < 85 ? "4" :
                                                   chance < 90 + effectiveAggression / 2 ? "6" : "W",

                _ => chance < 30 - effectiveAggression ? "0" :
                     chance < 60 ? "1" :
                     chance < 75 ? "2" :
                     chance < 85 ? "4" :
                     chance < 90 + effectiveAggression / 2 ? "6" : "W",
            };
        }

        public static int GetDefaultOvers(string gameType)
        {
            return gameType.ToUpper() switch
            {
                SimulationConstants.GameTypeTest => SimulationConstants.TestOvers,
                SimulationConstants.GameTypeODI => SimulationConstants.ODIOvers,
                SimulationConstants.GameTypeT20 => SimulationConstants.T20Overs,
                _ => SimulationConstants.T20Overs
            };
        }

        public static int ApplyBatterTypeModifier(string batterType, int aggression, int currentOver, int totalOvers)
        {
            return batterType switch
            {
                "Aggressive" => Math.Min(100, aggression + 15),
                "Anchor" => currentOver < 10 ? Math.Max(1, aggression - 10) : aggression,
                "Finisher" => currentOver >= totalOvers - 5 ? Math.Min(100, aggression + 20) : aggression,
                "Tailender" => Math.Max(1, aggression - 15),
                _ => aggression // AllRounder or unknown
            };
        }

    }
}
