using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Hubs;
using SignalRDemo.Utilities;

namespace SignalRDemo.Services;

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
        _busHub = busHub;
        _dashboardHub = dashboardHub;
        _store = store;
        _tracker = tracker;
        _groupControl = groupControl;

        _lines = new[]
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
            foreach (var line in _lines)
            {
                var state = _store.GetOrCreate(line.Group, line.Title, line.TotalStops);

                if (_rng.NextDouble() > 0.4)
                {
                    state.StopNumber++;
                    if (state.StopNumber > state.TotalStops)
                    {
                        state.StopNumber = 0;
                        ConsoleColors.WriteLine(ConsoleColor.Magenta, $"[Background] {state.Title} completed route. Restarting.");
                    }
                }

                state.ProgressPercent = state.TotalStops == 0 ? 0 : (double)state.StopNumber / state.TotalStops * 100.0;
                _store.Update(state);

                if (!_groupControl.IsPaused(state.Group))
                {
                    ConsoleColors.WriteLine(ConsoleColor.Blue, $"[Background] {state.Title}: Stop {state.StopNumber}/{state.TotalStops} ({state.ProgressPercent:F1}%)");
                    await _busHub.Clients.Group(line.Group).SendAsync("BusProgress", state, stoppingToken);
                }
                else
                {
                    ConsoleColors.WriteLine(ConsoleColor.DarkGray, $"[Background] {state.Title} broadcast paused");
                }
            }

            var dashboardData = _store.GetAll().Select(p => new
            {
                p.Group,
                p.Title,
                p.StopNumber,
                p.TotalStops,
                p.ProgressPercent,
                Users = _tracker.CountConnections(p.Group),
                Paused = _groupControl.IsPaused(p.Group),
                Connections = _tracker.ListGroupConnections(p.Group).Select(c => new { c.ConnectionId, c.ConnectedAtUtc })
            }).ToList();

            ConsoleColors.WriteLine(ConsoleColor.DarkYellow, "[Dashboard] Broadcast dashboard update");
            await _dashboardHub.Clients.All.SendAsync("DashboardUpdate", dashboardData, stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
