using Microsoft.AspNetCore.SignalR;
using SignalR_Mvc_BusDemo.Services;
using SignalR_Mvc_BusDemo.Utilities;

namespace SignalR_Mvc_BusDemo.Hubs
{
    public class DashboardHub : Hub
    {
        private readonly GroupControl _groupControl;
        private readonly IHubContext<BusHub> _busHub;

        public DashboardHub(GroupControl groupControl, IHubContext<BusHub> busHub)
        {
            this._groupControl = groupControl;
            this._busHub = busHub;
        }

        public Task PauseGroup(string group)
        {
            this._groupControl.Pause(group);
            ConsoleColors.WriteLine(ConsoleColor.DarkRed, $"[DashboardHub] Paused group {group}");
            return Task.CompletedTask;
        }

        public Task ResumeGroup(string group)
        {
            this._groupControl.Resume(group);
            ConsoleColors.WriteLine(ConsoleColor.Green, $"[DashboardHub] Resumed group {group}");
            return Task.CompletedTask;
        }

        public async Task Kick(string connectionId)
        {
            ConsoleColors.WriteLine(ConsoleColor.Red, $"[DashboardHub] Kick {connectionId}");
            await this._busHub.Clients.Client(connectionId).SendAsync("ForceDisconnect");
        }
    }
}
