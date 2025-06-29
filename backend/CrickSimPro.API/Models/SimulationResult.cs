namespace CrickSimPro.API.Models;

public class SimulationResult
{
    public required string Message { get; set; }
    public required string Pitch { get; set; }
    public required string Weather { get; set; }
    public int Runs { get; set; }
    public int Wickets { get; set; }
    public required List<List<string>> OversDetail { get; set; }
    public required List<OverStat> OverStats { get; set; } = [];
}
