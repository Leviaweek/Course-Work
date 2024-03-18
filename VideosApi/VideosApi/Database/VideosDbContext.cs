using Microsoft.EntityFrameworkCore;
using VideosApi.Models;

namespace VideosApi.Database;

public class VideosDbContext(DbContextOptions<VideosDbContext> options) : DbContext(options)
{
    public required DbSet<VideoInfo> VideosInfos { get; set; }
    public required DbSet<PhysicalVideo> PhysicalVideos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VideoInfo>()
            .HasOne(v => v.PhysicalVideo)
            .WithOne()
            .HasForeignKey<PhysicalVideo>(v => v.VideoInfoId)
            .HasPrincipalKey<VideoInfo>(p => p.Id);
    }
}