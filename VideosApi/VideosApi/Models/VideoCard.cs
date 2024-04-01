using System.Text.Json.Serialization;

namespace VideosApi.Models;

[Serializable]
public sealed class VideoCard
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    [JsonPropertyName("createdAt")]
    public required string CreatedAt { get; set; }
    [JsonPropertyName("duration")]
    public required long Duration { get; set; }
}