using System.Text.Json.Serialization;

namespace VideosApi.Models;

public class ConversionTaskDto
{
    public ConversionProgress? Progress { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ConversionState State { get; set; } = ConversionState.Pending;
}