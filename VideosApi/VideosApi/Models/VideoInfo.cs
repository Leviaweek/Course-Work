using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideosApi.Models;

[Table("VideosInfos", Schema = "public")]
public class VideoInfo
{
    [StringLength(32, MinimumLength = 32)]
    public required string Id { get; set; }
    [StringLength(100, MinimumLength = 1)]
    public required string Title { get; set; }

    public bool IsUploaded { get; set; }

    public PhysicalVideo PhysicalVideo { get; set; } = null!;
}
