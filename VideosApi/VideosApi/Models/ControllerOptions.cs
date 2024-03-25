using System.ComponentModel.DataAnnotations;

namespace VideosApi.Models;

public class ControllerOptions
{
    public const string OptionName = "ControllerOptions";
    [Required]
    public string FilesPath { get; set; } = string.Empty;
}