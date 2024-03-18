using System.Text.Json.Serialization;

namespace VideosApi.Models;

public class PostFileMetadata
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }
}