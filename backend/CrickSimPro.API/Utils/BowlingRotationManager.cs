namespace CrickSimPro.Utils
{
    public static class BowlingRotationManager
    {
        private static Dictionary<int, string> OverToBowlerMap = new();

        public static void Initialize(List<string> bowlerTypes)
        {
            OverToBowlerMap.Clear();
            if (bowlerTypes == null || bowlerTypes.Count == 0)
                throw new ArgumentException("BowlerTypes must not be empty");

            // Support up to 200 overs (very safe for multi-innings)
            for (int over = 1; over <= 200; over++)
            {
                int index = (over - 1) % bowlerTypes.Count;
                OverToBowlerMap[over] = bowlerTypes[index];
            }
        }

        public static string GetNextBowler(List<string> bowlerTypes, int overNumber, int totalOvers)
        {
            if (OverToBowlerMap.TryGetValue(overNumber, out var bowler))
                return bowler;

            // Fallback: round-robin
            return bowlerTypes[(overNumber - 1) % bowlerTypes.Count];
        }

        // You can call this before each innings if needed (not strictly required if Initialize is always called)
        public static void Reset()
        {
            OverToBowlerMap.Clear();
        }
    }
}
