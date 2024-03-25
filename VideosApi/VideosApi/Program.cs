using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using VideosApi.Database;
using VideosApi.Models;
using VideosApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


builder.Services.AddHostedService<ConversionQueueService>();
builder.Services.AddHostedService<ConversionCleaner>();
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = int.MaxValue; });
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = int.MaxValue; });
builder.Services.AddOptions<ControllerOptions>().Bind(builder.Configuration.GetSection(ControllerOptions.OptionName))
    .ValidateOnStart();
builder.Services.AddScoped<VideoHandler>();
builder.Services.AddDbContextFactory<VideosDbContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSingleton<ConversionQueue>();
var app = builder.Build();

app.UseRouting();
app.UseCors();

app.MapControllers();

app.Run();