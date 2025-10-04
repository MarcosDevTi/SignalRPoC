using System.Collections.Concurrent;

namespace SignalR_Mvc_BusDemo.Services
{
    public class GroupControl
    {
        private readonly ConcurrentDictionary<string, bool> _paused = new();
        public void Pause(string group) => this._paused[group] = true;
        public void Resume(string group) => this._paused[group] = false;
        public bool IsPaused(string group) => this._paused.TryGetValue(group, out var v) && v;
    }
}
