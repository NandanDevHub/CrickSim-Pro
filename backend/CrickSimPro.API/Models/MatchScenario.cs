namespace CrickSimPro.API.Models;

public class MatchScenario
{
    public required string PitchType { get; set; }
    public required string Weather { get; set; }
    public int BattingAggression { get; set; }
}
