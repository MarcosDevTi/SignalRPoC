using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRDemo.Hubs;
using SignalRDemo.Services;
using SignalRDemo.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<BusStateStore>();
builder.Services.AddSingleton<ConnectionTracker>();
builder.Services.AddSingleton<GroupControl>();
builder.Services.AddHostedService<BusBackgroundService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<BusHub>("/bushub");
app.MapHub<DashboardHub>("/dashboardhub");

ConsoleColors.WriteLine(ConsoleColor.Cyan, "[Startup] Application started. Navigate to /Home/Dashboard or /Home/Line/1..5");

app.Run();
