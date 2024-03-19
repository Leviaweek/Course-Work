using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideosApi.Database;
using VideosApi.Models;
using VideosApi.Services;
using System.IO;
using System.Net.Mime;

namespace VideosApi.Controllers;

[ApiController]
[Route("/api/videos")]
public class VideosController(
    ILogger<VideosController> logger,
    VideosDbContext dbContext,
    ConversionQueueService conversionQueueService,
    IServiceScopeFactory scopeFactory) : ControllerBase
{
    private const string FilesPath = @"C:\Users\leviaweek\Desktop\2314226";

    [HttpGet]
    public async Task<Ok<GetVideosResponse>> GetVideosListAsync(CancellationToken cancellationToken = default)
    {
        var videos = await dbContext.VideosInfos.Where(video => video.IsUploaded).OrderBy(video => video.Id).Take(10)
            .Select<VideoInfo, VideoCard>(video =>
                new VideoCard
                {
                    Id = video.Id,
                    Title = video.Title,
                    Duration = video.PhysicalVideo.Duration,
                    CreatedAt = video.PhysicalVideo.CreatedAt
                }).ToListAsync(cancellationToken: cancellationToken);
        var response = new GetVideosResponse
        {
            Count = videos.Count,
            Videos = videos
        };
        logger.LogInformation("Sent: {response}", response.ToString());
        return TypedResults.Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<Results<PhysicalFileHttpResult, NotFound>> GetVideoAsync(string id,
        CancellationToken cancellationToken = default)
    {
        var result =
            await dbContext.VideosInfos.FirstOrDefaultAsync(v => v.Id == id, cancellationToken: cancellationToken);
        if (result is null || !result.IsUploaded)
        {
            return TypedResults.NotFound();
        }

        logger.LogInformation("Sent video by id: {id}", id);
        return TypedResults.PhysicalFile(Path.Combine(FilesPath, result.Id, "video.mp4"), "video/mp4",
            fileDownloadName: $"{result.Id}.mp4", enableRangeProcessing: true);
    }

    [HttpGet("{id}/preview")]
    public async Task<Results<PhysicalFileHttpResult, NotFound>> GetPreviewAsync(string id,
        CancellationToken cancellationToken = default)
    {
        var result =
            await dbContext.VideosInfos.FirstOrDefaultAsync(v => v.Id == id, cancellationToken: cancellationToken);
        if (result is null || !result.IsUploaded)
        {
            return TypedResults.NotFound();
        }

        logger.LogInformation("Sent preview by id: {id}", id);
        return TypedResults.PhysicalFile(Path.Combine(FilesPath, result.Id, "preview.jpg"), "image/jpeg",
            fileDownloadName: $"{result.Id}.jpg", enableRangeProcessing: true);
    }

    [HttpPost]
    public async Task<Ok<string>> PostVideoMetadataAsync([FromForm] PostFileMetadata postFileMetadata,
        CancellationToken cancellationToken = default)
    {
        var video = new VideoInfo
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = postFileMetadata.Title,
        };
        await dbContext.VideosInfos.AddAsync(video, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(video.Id);
    }

    [HttpPost("{id}")]
    public async Task<Results<Ok<string>, NotFound<string>>> PostVideoAsync([FromForm] IFormFile formFile, string id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{info}", formFile.Name);
        logger.LogInformation("{info}", id);
        if (!formFile.ContentType.StartsWith("video"))
        {
            return TypedResults.NotFound("Not a video content");
        }

        var video = await dbContext.VideosInfos.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (video is null)
        {
            return TypedResults.NotFound("Video not found");
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var videoHandler = scope.ServiceProvider.GetRequiredService<VideoHandler>();
        var newPath = await videoHandler.SaveVideoAsync(formFile, cancellationToken);
        var task = new ConversionTask(newPath, Path.Combine(FilesPath, video.Id), video.Id);
        var taskId = conversionQueueService.AddTask(task);
        return TypedResults.Ok(taskId.ToString("N"));
    }

    [HttpGet("{id}/status")]
    public Results<Ok<ConversionTaskDto>, NotFound> GetVideoUploadingStatus(string id)
    {
        var task = conversionQueueService.GetTask(id);
        if (task is null)
        {
            return TypedResults.NotFound();
        }

        var dto = new ConversionTaskDto
        {
            Progress = task.Progress,
            State = task.State
        };
        return TypedResults.Ok(dto);
    }
}