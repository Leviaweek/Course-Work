using System.Collections.Concurrent;
using VideosApi.Models;

namespace VideosApi.Services;

public class ConversionQueue
{
    private readonly ConcurrentDictionary<Guid, ConversionTask> _conversionTasks = [];
    public Guid AddTask(ConversionTask task)
    {
        var id = Guid.NewGuid();
        _conversionTasks[id] = task;
        return id;
    }
    public ConversionTask? GetTask(Guid id)
    {
        return _conversionTasks.GetValueOrDefault(id);
    }
    public ConcurrentDictionary<Guid, ConversionTask> GetTasks()
    {
        return _conversionTasks;
    }
    public void RemoveTask(Guid id)
    {
        _conversionTasks.TryRemove(id, out _);
    }
}