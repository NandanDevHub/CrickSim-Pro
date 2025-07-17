namespace CrickSimPro.Constants
{
    public static class SimulationConstants
    {
        // Ball and over info
        public const int BallsPerOver = 6;

        // Default overs by game type
        public const int TestOvers = 90;
        public const int ODIOvers = 50;
        public const int T20Overs = 20;

        // Game types
        public const string GameTypeTest = "TEST";
        public const string GameTypeODI = "ODI";
        public const string GameTypeT20 = "T20";

        // Pitch types
        public const string PitchGreen = "green";
        public const string PitchDry = "dry";
        public const string PitchNormal = "normal";

        // Weather types
        public const string WeatherSunny = "sunny";
        public const string WeatherCloudy = "cloudy";
        public const string WeatherHumid = "humid";

        // Bowler types
        public const string BowlerSpin = "spin";
        public const string BowlerPace = "pace";

        // Stamina
        public const int MaxStamina = 100;
        public const int StaminaLossPerOver = 8;
        public const int StaminaLossPerBall = 2;
        public const int StaminaLossPerRun = 1;

        // Pressure Modifiers
        public const int PressurePerWicket = 6;
        public const double PressurePerRRGap = 5.0; // per 1 run rate difference

        public const int AnchorVsSpinPenalty = -3;
        public const int AggressiveVsSwingPenalty = -2;
        public const int FinisherVsSpinBoost = 2;

        public const int CloudyWeatherSwingBonus = 3;
        public const int DryWeatherSpinBonus = 3;

        // Matchup Modifiers
        public const int MATCHUP_PACE_VS_ANCHOR_BONUS = 5;
        public const int MATCHUP_SPIN_VS_AGGRESSIVE_BONUS = 5;
        public const int MATCHUP_SWING_VS_TAILENDER_BONUS = 10;

        // Weather Bonus/Penalty
        public const int WEATHER_CLOUDY_SWING_BONUS = 5;
        public const int WEATHER_DRY_SPIN_BONUS = 3;
        public const int WEATHER_NORMAL_PACE_BONUS = 2;

    }
}
