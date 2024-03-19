using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using VideosApi.Database;
using VideosApi.Models;

namespace VideosApi.Services;

public class ConversionQueueService(IServiceScopeFactory serviceScopeFactory, IDbContextFactory<VideosDbContext> dbContextFactory)
{
    private readonly ConcurrentDictionary<Guid, ConversionTask> _conversionTasks = [];
    private DateTimeOffset _lastClearTime = DateTimeOffset.MinValue;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var task = _conversionTasks.Values.OrderBy(task => task.CreatedAt).FirstOrDefault(task => task.State == ConversionState.Pending);
            if (task is not null)
            {
                await HandleTaskAsync(task, cancellationToken);
            }
            if ((DateTimeOffset.UtcNow - _lastClearTime).TotalDays >= 1)
            {
                ClearOldTasks();
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
                Size = new FileInfo(Path.Combine(task.OutputPath, "video.mp4")).Length,
                Duration = task.Duration,
                CreatedAt = DateTimeOffset.UtcNow.ToString()
            };
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private void ClearOldTasks()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var task in _conversionTasks.Where(task => 
                     task.Value.State == ConversionState.Completed || task.Value.State == ConversionState.Failed && (now - task.Value.CreatedAt).TotalDays >= 1))
        {
            _conversionTasks.TryRemove(task.Key, out _);
        }
        _lastClearTime = now;
    }
    
    
    public ConversionTask? GetTask(string id)
    {
        var result = _conversionTasks.TryGetValue(new Guid(id), out var conversionTask);
        return result ? conversionTask : null;
    }

    public Guid AddTask(ConversionTask task)
    {
        var guid = Guid.NewGuid();
        _conversionTasks[guid] = task;
        return guid;
    }
}