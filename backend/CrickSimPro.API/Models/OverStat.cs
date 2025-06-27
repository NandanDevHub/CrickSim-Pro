namespace CrickSimPro.API.Models;

public class OverStat
{
public int OverNumber { get; set; }
    public string Bowler { get; set; } = string.Empty;
    public List<string> Deliveries { get; set; } = new();
    public int Runs { get; set; }
    public int Wickets { get; set; }
}