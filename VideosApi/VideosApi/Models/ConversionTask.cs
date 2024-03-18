using System.Text.Json.Serialization;
using Xabe.FFmpeg;

namespace VideosApi.Models;

public class ConversionTask(string tempPath, string outputPath, string videoId)
{
    public string OutputPath { get; set; } = outputPath;
    public string VideoId { get; set; } = videoId;
    public string TempPath { get; set; } = tempPath;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ConversionProgress? Progress { get; set; }
    public ConversionState State { get; set; } = ConversionState.Pending;
    public long Duration { get; set; }
}   