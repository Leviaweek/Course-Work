namespace VideosApi.Services;

public class ConversionQueueHostedService(ConversionQueueService conversionQueueService): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await conversionQueueService.StartAsync(stoppingToken);
    }
}