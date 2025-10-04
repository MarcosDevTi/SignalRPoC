using Microsoft.AspNetCore.SignalR;
using SignalR_Mvc_BusDemo.Services;
using SignalR_Mvc_BusDemo.Utilities;

namespace SignalR_Mvc_BusDemo.Hubs
{
    public class BusHub : Hub
    {
        private readonly ConnectionTracker _tracker;

        public BusHub(ConnectionTracker tracker)
        {
            this._tracker = tracker;
        }

        public async Task JoinGroup(string groupName)
        {
            ConsoleColors.WriteLine(ConsoleColor.Green, $"[BusHub] Connection {this.Context.ConnectionId} joined group {groupName}");
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
            this._tracker.AddConnection(groupName, this.Context.ConnectionId);
        }

        public async Task LeaveGroup(string groupName)
        {
            ConsoleColors.WriteLine(ConsoleColor.Yellow, $"[BusHub] Connection {this.Context.ConnectionId} left group {groupName}");
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
            this._tracker.RemoveConnection(groupName, this.Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConsoleColors.WriteLine(ConsoleColor.Red, $"[BusHub] Connection {this.Context.ConnectionId} disconnected");
            this._tracker.RemoveConnectionFromAll(this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
