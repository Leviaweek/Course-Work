using System.Text.Json.Serialization;

namespace VideosApi.Models;

[Serializable]
public class GetVideosResponse
{
    public required int Count { get; set; }
    public required List<VideoCard> Videos { get; set; }
}