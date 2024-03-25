using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideosApi.Database;
using VideosApi.Models;
using VideosApi.Services;
using Microsoft.Extensions.Options;

namespace VideosApi.Controllers;

[ApiController]
[Route("/api/videos")]
public class VideosController(
    ILogger<VideosController> logger,
    VideosDbContext dbContext,
    ConversionQueue conversionQueue,
    IOptions<ControllerOptions> options,
    VideoHandler videoHandler) : ControllerBase
{
    private readonly string _filesPath = options.Value.FilesPath;

    [HttpGet]
    public async Task<Ok<GetVideosResponse>> GetVideosListAsync(CancellationToken cancellationToken = default)
    {
        var videos = await dbContext.VideosInfos.Where(video => video.IsUploaded)
            .OrderBy(video => video.PhysicalVideo.CreatedAt).Take(10)
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
        return TypedResults.PhysicalFile(Path.Combine(_filesPath, result.Id, Constants.VideoFileName), "video/mp4",
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
        return TypedResults.PhysicalFile(Path.Combine(_filesPath, result.Id, Constants.PreviewFileName), "image/jpeg",
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

        var newPath = await videoHandler.SaveVideoAsync(formFile, cancellationToken);
        var task = new ConversionTask(newPath, Path.Combine(_filesPath, video.Id), video.Id);
        var taskId = conversionQueue.AddTask(task);
        return TypedResults.Ok(taskId.ToString("N"));
    }

    [HttpGet("{id}/status")]
    public Results<Ok<ConversionTaskDto>, NotFound> GetVideoUploadingStatus(string id)
    {
        if (Guid.TryParse(id, out var guid))
        {
            return TypedResults.NotFound();
        }

        var task = conversionQueue.GetTask(guid);
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