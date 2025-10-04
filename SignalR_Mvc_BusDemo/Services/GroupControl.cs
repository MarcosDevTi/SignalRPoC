using System.Collections.Concurrent;

namespace SignalRDemo.Services;

public class GroupControl
{
    private readonly ConcurrentDictionary<string, bool> _paused = new();
    public void Pause(string group) => _paused[group] = true;
    public void Resume(string group) => _paused[group] = false;
    public bool IsPaused(string group) => _paused.TryGetValue(group, out var v) && v;
}
