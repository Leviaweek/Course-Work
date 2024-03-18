namespace VideosApi.Models;

public class GetPreviewResponse
{
    public string? Error { get; }
    public bool IsSuccessful { get; }

    public GetPreviewResponse(string error)
    {
        Error = error;
        IsSuccessful = false;
    }
    public GetPreviewResponse()
    {
        IsSuccessful = true;
    }
}