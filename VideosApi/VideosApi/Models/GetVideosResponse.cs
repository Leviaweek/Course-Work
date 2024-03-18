namespace VideosApi.Models;

public record GetVideosResponse(int Count, List<VideoCard> Videos);