using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideosApi.Models;

[Serializable]
[Table("PhysicalVideos", Schema = "public")]
public class PhysicalVideo
{
    [StringLength(32, MinimumLength = 32)]
    [Key]
    public required string VideoInfoId { get; set; }
    
    public required long Size { get; set; }
    public required long Duration { get; set; }
    [StringLength(50)]
    public required string CreatedAt { get; set; }
}