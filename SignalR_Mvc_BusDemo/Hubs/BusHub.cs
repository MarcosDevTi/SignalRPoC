using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Services;
using SignalRDemo.Utilities;

namespace SignalRDemo.Hubs;

public class BusHub : Hub
{
    private readonly ConnectionTracker _tracker;
    private readonly BusStateStore _store;

    public BusHub(ConnectionTracker tracker, BusStateStore store)
    {
        _tracker = tracker;
        _store = store;
    }

    public async Task JoinGroup(string groupName)
    {
        ConsoleColors.WriteLine(ConsoleColor.Green, $"[BusHub] Connection {Context.ConnectionId} joined group {groupName}");
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _tracker.AddConnection(groupName, Context.ConnectionId);
    }

    public async Task LeaveGroup(string groupName)
    {
        ConsoleColors.WriteLine(ConsoleColor.Yellow, $"[BusHub] Connection {Context.ConnectionId} left group {groupName}");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _tracker.RemoveConnection(groupName, Context.ConnectionId);
    }

    public Task<Models.BusProgress?> GetCurrent(string groupName)
    {
        if (_store.TryGet(groupName, out var p)) return Task.FromResult<Models.BusProgress?>(p);
        return Task.FromResult<Models.BusProgress?>(null);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConsoleColors.WriteLine(ConsoleColor.Red, $"[BusHub] Connection {Context.ConnectionId} disconnected");
        _tracker.RemoveConnectionFromAll(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }


}