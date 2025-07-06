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

    }
}
