using System;
using System.Collections.Generic;

namespace CrickSimPro.Utils
{
    public static class BowlingRotationManager
    {
        private static Dictionary<int, string> OverToBowlerMap = new();

        public static void Initialize(List<string> bowlerNames)
        {
            OverToBowlerMap.Clear();
            if (bowlerNames == null || bowlerNames.Count == 0)
                throw new ArgumentException("Bowler names must not be empty");

            string lastBowler = null;
            int bowlerIndex = 0;
            for (int over = 1; over <= 200; over++)
            {
                if (bowlerNames.Count == 1)
                {
                    OverToBowlerMap[over] = bowlerNames[0];
                    continue;
                }

                int attempts = 0;
                while (attempts < bowlerNames.Count)
                {
                    string candidate = bowlerNames[bowlerIndex % bowlerNames.Count];
                    bowlerIndex++;
                    if (candidate != lastBowler)
                    {
                        OverToBowlerMap[over] = candidate;
                        lastBowler = candidate;
                        break;
                    }
                    attempts++;
                }
                if (!OverToBowlerMap.ContainsKey(over))
                    OverToBowlerMap[over] = bowlerNames[(over - 1) % bowlerNames.Count];
            }
        }

        public static string GetNextBowler(List<string> bowlerNames, int overNumber, int totalOvers)
        {
            if (OverToBowlerMap.TryGetValue(overNumber, out var bowler))
                return bowler;
            return bowlerNames.Count > 0
                ? bowlerNames[(overNumber - 1) % bowlerNames.Count]
                : throw new ArgumentException("No bowlers available");
        }

        public static void Reset()
        {
            OverToBowlerMap.Clear();
        }
    }
}
