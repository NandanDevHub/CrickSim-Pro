namespace CrickSimPro.Utils;

public static class BowlingRotationManager
{
    private static Dictionary<int, string> OverToBowlerMap = new();

    public static void Initialize(List<string> bowlerTypes)
    {
        OverToBowlerMap.Clear();
        for (int over = 1; over <= 100; over++)  // support up to 100 overs
        {
            int index = (over - 1) % bowlerTypes.Count;
            OverToBowlerMap[over] = bowlerTypes[index];
        }
    }

    public static string GetNextBowler(List<string> bowlerTypes, int overNumber, int totalOvers)
    {
        if (OverToBowlerMap.TryGetValue(overNumber, out var bowler))
            return bowler;

        // fallback: round-robin
        return bowlerTypes[(overNumber - 1) % bowlerTypes.Count];
    }
}
