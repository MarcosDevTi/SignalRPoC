using SignalR_Mvc_BusDemo.Models;
using System.Collections.Concurrent;

namespace SignalR_Mvc_BusDemo.Services
{
    public class BusStateStore
    {
        private readonly ConcurrentDictionary<string, BusProgress> _states = new();

        public BusProgress GetOrCreate(string group, string title, int totalStops)
        {
            return this._states.GetOrAdd(group, _ => new BusProgress
            {
                Group = group,
                Title = title,
                StopNumber = 0,
                TotalStops = totalStops,
                ProgressPercent = 0,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        public void Update(BusProgress p)
        {
            p.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this._states[p.Group] = p;
        }

        public IEnumerable<BusProgress> GetAll() => this._states.Values.OrderBy(v => v.Title);
    }
}
