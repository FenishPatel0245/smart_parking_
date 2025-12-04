using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;
using SmartParkingLot.Infrastructure.Data;
using SmartParkingLot.Infrastructure.Repositories;
using SmartParkingLot.Application.Services;
using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Application.ViewModels;
using SmartParkingLot.Web.Hubs;
using SmartParkingLot.Web.Hubs;
using SmartParkingLot.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddSignalR();
builder.Services.AddScoped<ProtectedSessionStorage>();

// Database
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "scada_monitoring.db");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Repositories
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IEventLogRepository, EventLogRepository>();
builder.Services.AddScoped<IParkingTransactionRepository, ParkingTransactionRepository>();

// Design Patterns
builder.Services.AddSingleton<IDeviceFactory, DeviceFactory>();
builder.Services.AddSingleton<IDeviceSubject, DeviceSubject>();
builder.Services.AddSingleton<IAlertStrategy, ThresholdAlertStrategy>();

// Services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IDeviceManagementService, DeviceManagementService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IDeviceControlService, DeviceControlService>();
builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.AddScoped<ISimulationService, SimulationService>();
builder.Services.AddScoped<IBroadcastService, SmartParkingLot.Web.Services.SignalRBroadcastService>();
builder.Services.AddScoped<ISessionStorageService, SessionStorageService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();


// ViewModels
builder.Services.AddScoped<DashboardViewModel>();
builder.Services.AddScoped<SimulationViewModel>();

builder.Services.AddSingleton<ISystemStateService, SystemStateService>();

// Health Checks
// Adds health check services to the container to monitor application health
builder.Services.AddHealthChecks();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Authentication & Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.MapBlazorHub();
app.MapHub<TelemetryHub>("/telemetryhub");

// Health Check Endpoint
// Maps the health check endpoint to /health for external monitoring
app.MapHealthChecks("/health");

app.MapFallbackToPage("/_Host");

app.Run();
