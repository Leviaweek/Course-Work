namespace VideosApi.Models;

[Serializable]
public record ConversionProgress
{
    public double Speed { get; set; }
    public DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.MinValue;
    public int Percent { get; set; }
    public double Duration { get; set; }
    public double TimeLeft { get; set; } 
}