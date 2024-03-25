using VideosApi.Models;

namespace VideosApi.Services;

public class ConversionCleaner(ILogger<ConversionQueue> logger, ConversionQueue conversionQueue) : BackgroundService
{
    private DateTimeOffset _lastClearTime = DateTimeOffset.MinValue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if ((DateTimeOffset.UtcNow - _lastClearTime).TotalDays >= 1)
            {
                ClearOldTasks();
                logger.LogInformation("Cleared old tasks");
            }

            await Task.Delay(1000 * 60, stoppingToken);
        }
    }

    private void ClearOldTasks()
    {
        var now = DateTimeOffset.UtcNow;
        var tasks = conversionQueue.GetTasks();
        foreach (var task in tasks.Where(task =>
                     task.Value.State == ConversionState.Completed || task.Value.State == ConversionState.Failed &&
                     (now - task.Value.CreatedAt).TotalDays >= 1))
        {
            conversionQueue.RemoveTask(task.Key);
        }

        _lastClearTime = now;
    }
}