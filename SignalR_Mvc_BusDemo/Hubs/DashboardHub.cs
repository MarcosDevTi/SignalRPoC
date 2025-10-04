using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Services;
using SignalRDemo.Utilities;

namespace SignalRDemo.Hubs;

public class DashboardHub : Hub
{
    private readonly GroupControl _groupControl;
    private readonly IHubContext<BusHub> _busHub;

    public DashboardHub(GroupControl groupControl, IHubContext<BusHub> busHub)
    {
        _groupControl = groupControl;
        _busHub = busHub;
    }

    public Task PauseGroup(string group)
    {
        _groupControl.Pause(group);
        ConsoleColors.WriteLine(ConsoleColor.DarkRed, $"[DashboardHub] Paused group {group}");
        return Task.CompletedTask;
    }

    public Task ResumeGroup(string group)
    {
        _groupControl.Resume(group);
        ConsoleColors.WriteLine(ConsoleColor.Green, $"[DashboardHub] Resumed group {group}");
        return Task.CompletedTask;
    }

    public async Task Kick(string connectionId)
    {
        ConsoleColors.WriteLine(ConsoleColor.Red, $"[DashboardHub] Kick {connectionId}");
        await _busHub.Clients.Client(connectionId).SendAsync("ForceDisconnect");
    }
}
