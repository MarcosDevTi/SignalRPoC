namespace SignalRDemo.Models;

public class BusProgress
{
    public string Group { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int StopNumber { get; set; }
    public int TotalStops { get; set; }
    public double ProgressPercent { get; set; }
    public long Timestamp { get; set; }
}
