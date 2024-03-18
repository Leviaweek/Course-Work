using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using VideosApi.Models;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Exceptions;

namespace VideosApi.Services;

public class VideoHandler(ILogger<VideoHandler> logger)
{
    public async Task<bool> ConvertToMp4Async(ConversionTask task, CancellationToken cancellationToken = default)
    {
        var videoStream = await FFmpeg.GetMediaInfo(task.TempPath, cancellationToken);
        task.Duration = (long)videoStream.Duration.TotalMilliseconds;
        logger.LogInformation("{}", videoStream.Size);
        try
        {
            var res = FFmpeg.Conversions.New()
                .AddStream(videoStream.Streams)
                .AddParameter("-c:v libx264")
                .SetOutput(Path.Combine(task.OutputPath, "video.mp4"));
            
            res.OnProgress += (sender, args) =>
            {
                task.Progress ??= new ConversionProgress();
                task.Progress.Percent = args.Percent;
                var speed = (args.Duration.TotalSeconds - task.Progress.Duration) / (DateTimeOffset.UtcNow - task.Progress.LastUpdate).TotalSeconds;
                if (!double.IsInfinity(speed))
                {
                    task.Progress.Speed = speed;
                }
                task.Progress.LastUpdate = DateTimeOffset.UtcNow;
                task.Progress.Duration = args.Duration.TotalSeconds;
                task.Progress.TimeLeft = (args.TotalLength.TotalSeconds - args.Duration.TotalSeconds) / task.Progress.Speed;
            };
            
            await res.Start(cancellationToken);
            
            logger.LogInformation("{}", res);
            return true;
        }
        catch (Exception ex) when (ex is ConversionException or InvalidInputException)
        {
            logger.LogInformation("Error: {}", ex);
            return false;
        }
    }

    public async Task<bool> CreatePreviewAsync(string videoPath, CancellationToken cancellationToken = default)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var mediaInfo = await FFmpeg.GetMediaInfo(Path.Combine(videoPath, "video.mp4"), cancellationToken);
        try
        {
            await FFmpeg.Conversions.New()
                .AddStream(mediaInfo.VideoStreams.First())
                .AddParameter("-s 1280x720")
                .AddParameter($"-ss {(mediaInfo.Duration / 4).ToFFmpeg()}")
                .SetOutput(Path.Combine(videoPath, "preview.jpg"))
                .SetOutputFormat(Format.mjpeg)
                .Start(cancellationToken);
            stopWatch.Stop();
            logger.LogInformation("Conversion took: {time}", stopWatch.Elapsed);
            return true;
        }
        catch (Exception ex) when (ex is ConversionException or InvalidInputException)
        {
            stopWatch.Stop();
            logger.LogInformation("Error: {}", ex);
            return false;
        }
    }

    public async Task<string> SaveVideoAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        await using var inputStream = formFile.OpenReadStream();
        var tempPath = Path.GetTempFileName();
        var newPath = tempPath + Path.GetExtension(formFile.FileName);
        File.Move(tempPath, newPath);
        await using var outputStream = new FileStream(newPath, FileMode.Open);
        await inputStream.CopyToAsync(outputStream, cancellationToken);
        return newPath;
    }
}