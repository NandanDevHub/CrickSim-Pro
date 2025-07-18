namespace CrickSimPro.Constants
{
    public static class SimulationConstants
    {
        // --- Balls & Overs ---
        public const int BallsPerOver = 6;

        // --- Default overs for game types ---
        public const int TestOvers = 90;
        public const int ODIOvers = 50;
        public const int T20Overs = 20;

        // --- Game Types (UPPERCASE for logic, lowercase for JSON/binding) ---
        public const string GameTypeTest = "TEST";
        public const string GameTypeODI = "ODI";
        public const string GameTypeT20 = "T20";

        // --- Pitch types (always lowercase in requests and code) ---
        public const string PitchGreen = "green";
        public const string PitchDry = "dry";
        public const string PitchNormal = "normal";

        // --- Weather types ---
        public const string WeatherSunny = "sunny";
        public const string WeatherCloudy = "cloudy";
        public const string WeatherHumid = "humid";
        public const string WeatherDry = "dry";  

        // --- Bowler types (ALWAYS lowercase for all matching!) ---
        public const string BowlerSpin = "spin";
        public const string BowlerPace = "pace";
        public const string BowlerSwing = "swing";

        // --- Stamina & Fatigue ---
        public const int MaxStamina = 100;
        public const int StaminaLossPerOver = 8;
        public const int StaminaLossPerBall = 2;
        public const int StaminaLossPerRun = 1;

        // --- Pressure/Chase Modifiers ---
        public const int PressurePerWicket = 6;
        public const double PressurePerRRGap = 5.0; // per 1 run rate difference

        // --- Skill-Based Matchup Modifiers ---
        public const int AnchorVsSpinPenalty = -3;
        public const int AggressiveVsSwingPenalty = -2;
        public const int FinisherVsSpinBoost = 2;

        // --- Weather-based Bowler Bonuses ---
        public const int CloudyWeatherSwingBonus = 3;
        public const int DryWeatherSpinBonus = 3;

        // --- Advanced Matchup Modifiers (for future) ---
        public const int MatchupPaceVsAnchorBonus = 5;
        public const int MatchupSpinVsAggressiveBonus = 5;
        public const int MatchupSwingVsTailenderBonus = 10;

        // --- Randomness ---
        public const int MinRandomAggressionAdjust = -5;
        public const int MaxRandomAggressionAdjust = 5;

        // --- Multi-innings support ---
        public const int MaxInnings = 2;

        // --- UI/Report Friendly Arrays (optional but useful for dropdowns and checks) ---
        public static readonly string[] AllBowlerTypes = { BowlerPace, BowlerSwing, BowlerSpin };
        public static readonly string[] AllPitchTypes = { PitchGreen, PitchDry, PitchNormal };
        public static readonly string[] AllWeatherTypes = { WeatherSunny, WeatherCloudy, WeatherHumid };
    }
}
