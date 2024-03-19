using System.Text.Json.Serialization;

namespace VideosApi.Models;

[Serializable]
public class PostFileMetadata
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }
}