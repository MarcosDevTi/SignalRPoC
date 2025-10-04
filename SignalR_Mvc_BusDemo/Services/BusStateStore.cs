using SignalRDemo.Models;
using System.Collections.Concurrent;

namespace SignalRDemo.Services;

public class BusStateStore
{
    private readonly ConcurrentDictionary<string, BusProgress> _states = new();

    public BusProgress GetOrCreate(string group, string title, int totalStops)
    {
        return _states.GetOrAdd(group, _ => new BusProgress
        {
            Group = group,
            Title = title,
            StopNumber = 0,
            TotalStops = totalStops,
            ProgressPercent = 0,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    public bool TryGet(string group, out BusProgress? progress) => _states.TryGetValue(group, out progress);

    public void Update(BusProgress p)
    {
        p.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _states[p.Group] = p;
    }

    public IEnumerable<BusProgress> GetAll() => _states.Values.OrderBy(v => v.Title);


}