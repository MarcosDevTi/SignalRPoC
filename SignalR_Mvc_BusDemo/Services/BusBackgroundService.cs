using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SignalR_Mvc_BusDemo.Hubs;
using SignalR_Mvc_BusDemo.Models;
using SignalR_Mvc_BusDemo.Utilities;

namespace SignalR_Mvc_BusDemo.Services
{
    public class BusBackgroundService : BackgroundService
    {
        private readonly IHubContext<BusHub> _busHub;
        private readonly IHubContext<DashboardHub> _dashboardHub;
        private readonly BusStateStore _store;
        private readonly ConnectionTracker _tracker;
        private readonly GroupControl _groupControl;
        private readonly (string Group, string Title, int TotalStops)[] _lines;
        private readonly Random _rng = new();

        public BusBackgroundService(
            IHubContext<BusHub> busHub,
            IHubContext<DashboardHub> dashboardHub,
            BusStateStore store,
            ConnectionTracker tracker,
            GroupControl groupControl)
        {
            this._busHub = busHub;
            this._dashboardHub = dashboardHub;
            this._store = store;
            this._tracker = tracker;
            this._groupControl = groupControl;

            this._lines = new[]
            {
                ("group-a", "Line A", 12),
                ("group-b", "Line B", 15),
                ("group-c", "Line C", 18),
                ("group-d", "Line D", 10),
                ("group-e", "Line E", 20)
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConsoleColors.WriteLine(ConsoleColor.Cyan, "[Background] BusBackgroundService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var line in this._lines)
                {
                    var state = this._store.GetOrCreate(line.Group, line.Title, line.TotalStops);

                    if (this._rng.NextDouble() > 0.4)
                    {
                        state.StopNumber++;
                        if (state.StopNumber > state.TotalStops)
                        {
                            state.StopNumber = 0;
                            ConsoleColors.WriteLine(ConsoleColor.Magenta, $"[Background] {state.Title} completed route. Restarting.");
                        }
                    }

                    state.ProgressPercent = state.TotalStops == 0 ? 0 : (double)state.StopNumber / state.TotalStops * 100.0;
                    this._store.Update(state);

                    if (!this._groupControl.IsPaused(state.Group))
                    {
                        ConsoleColors.WriteLine(ConsoleColor.Blue, $"[Background] {state.Title}: Stop {state.StopNumber}/{state.TotalStops} ({state.ProgressPercent:F1}%)");
                        await this._busHub.Clients.Group(line.Group).SendAsync("BusProgress", state, stoppingToken);
                    }
                    else
                    {
                        ConsoleColors.WriteLine(ConsoleColor.DarkGray, $"[Background] {state.Title} broadcast paused");
                    }
                }

                var dashboardData = this._store.GetAll().Select(p => new
                {
                    p.Group,
                    p.Title,
                    p.StopNumber,
                    p.TotalStops,
                    p.ProgressPercent,
                    Users = this._tracker.CountConnections(p.Group),
                    Paused = this._groupControl.IsPaused(p.Group),
                    Connections = this._tracker.ListGroupConnections(p.Group).Select(c => new { c.ConnectionId, ConnectedAtUtc = c.ConnectedAtUtc })
                }).ToList();

                ConsoleColors.WriteLine(ConsoleColor.DarkYellow, "[Dashboard] Broadcast dashboard update");
                await this._dashboardHub.Clients.All.SendAsync("DashboardUpdate", dashboardData, stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
