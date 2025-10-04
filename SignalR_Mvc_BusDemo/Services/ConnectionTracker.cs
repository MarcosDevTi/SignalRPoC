using System.Collections.Concurrent;

namespace SignalRDemo.Services;

public class ConnectionTracker
{
    public class ConnectionInfo
    {
        public string ConnectionId { get; set; } = string.Empty;
        public HashSet<string> Groups { get; set; } = new();
        public DateTime ConnectedAtUtc { get; set; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<string, ConnectionInfo> _connections = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _groupConnections = new();

    public void AddConnection(string group, string connectionId)
    {
        var info = _connections.GetOrAdd(connectionId, id => new ConnectionInfo { ConnectionId = id, ConnectedAtUtc = DateTime.UtcNow });
        lock (info) { info.Groups.Add(group); }
        var set = _groupConnections.GetOrAdd(group, _ => new HashSet<string>());
        lock (set) { set.Add(connectionId); }
    }

    public void RemoveConnection(string group, string connectionId)
    {
        if (_connections.TryGetValue(connectionId, out var info))
        {
            lock (info) { info.Groups.Remove(group); }
        }
        if (_groupConnections.TryGetValue(group, out var set))
        {
            lock (set) { set.Remove(connectionId); }
        }
    }

    public void RemoveConnectionFromAll(string connectionId)
    {
        if (_connections.TryRemove(connectionId, out var info))
        {
            foreach (var g in info.Groups)
            {
                if (_groupConnections.TryGetValue(g, out var set))
                {
                    lock (set) { set.Remove(connectionId); }
                }
            }
        }
        else
        {
            foreach (var kvp in _groupConnections)
            {
                lock (kvp.Value) { kvp.Value.Remove(connectionId); }
            }
        }
    }

    public int CountConnections(string group) =>
        _groupConnections.TryGetValue(group, out var set) ? set.Count : 0;

    public IEnumerable<(string Group, string ConnectionId, DateTime ConnectedAtUtc)> ListGroupConnections(string group)
    {
        if (!_groupConnections.TryGetValue(group, out var set)) yield break;
        foreach (var id in set)
        {
            if (_connections.TryGetValue(id, out var info))
            {
                yield return (group, id, info.ConnectedAtUtc);
            }
        }
    }
}
