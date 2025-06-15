namespace CrickSimPro.API.Models;

public class MatchScenario
{
    public required string PitchType { get; set; }
    public required string Weather { get; set; }
    public int BattingAggression { get; set; }

    public required string BowlerType { get; set; }
    public int BowlingAggression { get; set; }

    public int Overs { get; set; }

    public required string GameType { get; set; }

}
