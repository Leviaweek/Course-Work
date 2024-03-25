using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using VideosApi.Database;
using VideosApi.Models;

namespace VideosApi.Services;

public class ConversionQueueService(
    IServiceScopeFactory serviceScopeFactory,
    IDbContextFactory<VideosDbContext> dbContextFactory,
    ConversionQueue conversionQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var tasks = conversionQueue.GetTasks();
            var task = tasks.Values.OrderBy(task => task.CreatedAt)
                .FirstOrDefault(task => task.State == ConversionState.Pending);
            if (task is not null)
            {
                await HandleTaskAsync(task, cancellationToken);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    private async Task HandleTaskAsync(ConversionTask task, CancellationToken cancellationToken)
    {
        task.State = ConversionState.InProgress;
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var videoHandler = scope.ServiceProvider.GetRequiredService<VideoHandler>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var result = await videoHandler.ConvertToMp4Async(task, cancellationToken);
        File.Delete(task.TempPath);
        if (!result)
        {
            task.State = ConversionState.Failed;
            return;
        }

        var previewResult = await videoHandler.CreatePreviewAsync(task.OutputPath, cancellationToken);
        task.State = previewResult ? ConversionState.Completed : ConversionState.Failed;
        var videoInfo = await dbContext.VideosInfos.FirstOrDefaultAsync(v => v.Id == task.VideoId, cancellationToken);
        if (videoInfo is not null)
        {
            videoInfo.IsUploaded = true;
            videoInfo.PhysicalVideo = new PhysicalVideo
            {
                VideoInfoId = videoInfo.Id,
                Size = new FileInfo(Path.Combine(task.OutputPath, Constants.VideoFileName)).Length,
                Duration = task.Duration,
                CreatedAt = DateTimeOffset.UtcNow.ToString()
            };
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}