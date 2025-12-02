using Microsoft.AspNetCore.SignalR;
using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Application.DTOs;

namespace SmartParkingLot.Web.Hubs;

/// <summary>
/// SignalR hub for real-time telemetry updates
/// </summary>
public class TelemetryHub : Hub, IDeviceObserver
{
    private static IHubContext<TelemetryHub>? _hubContext;

    public TelemetryHub(IHubContext<TelemetryHub> hubContext, IDeviceSubject deviceSubject)
    {
        _hubContext = hubContext;
        
        // Register as observer
        deviceSubject.Attach(this);
    }

    public async Task OnTelemetryUpdated(TelemetryDto telemetry)
    {
        if (_hubContext != null)
        {
            await _hubContext.Clients.All.SendAsync("TelemetryUpdate", telemetry);
        }
    }

    public async Task OnDeviceStatusChanged(DeviceDto device)
    {
        if (_hubContext != null)
        {
            await _hubContext.Clients.All.SendAsync("DeviceStatusChanged", device);
        }
    }

    public async Task OnAlertGenerated(AlertDto alert)
    {
        if (_hubContext != null)
        {
            await _hubContext.Clients.All.SendAsync("AlertGenerated", alert);
        }
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
