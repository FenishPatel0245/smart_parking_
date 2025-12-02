using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartParkingLot.Application.ViewModels;
using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Application.Services;

public class AutomationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutomationService> _logger;
    private readonly ISystemStateService _systemState;

    public AutomationService(
        IServiceProvider serviceProvider,
        ILogger<AutomationService> logger,
        ISystemStateService systemState)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _systemState = systemState;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_systemState.IsAutoModeEnabled)
                {
                    await CheckAndControlLights();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AutomationService");
            }

            await Task.Delay(2000, stoppingToken); // Check every 2 seconds
        }
    }

    private async Task CheckAndControlLights()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceManagementService>();
            var controlService = scope.ServiceProvider.GetRequiredService<IDeviceControlService>();

            var devices = await deviceService.GetAllDevicesAsync();
            var lights = devices.Where(d => d.Type == "SmartLighting").ToList();
            
            // Logic: Turn ON if > 6 PM OR any slot occupied
            // We need to fetch slots here since we don't have ViewModel
            var slots = await deviceService.GetParkingSlotsAsync();
            
            var now = DateTime.Now;
            bool isNightTime = now.Hour >= 18 || now.Hour < 6;
            bool isOccupied = slots.Any(s => s.IsOccupied);

            bool shouldBeOn = isNightTime || isOccupied;

            foreach (var light in lights)
            {
                // Only send command if state needs changing to avoid spamming
                bool isOn = light.CurrentValue > 0;
                
                if (shouldBeOn && !isOn)
                {
                    await controlService.ControlDeviceAsync(new DTOs.DeviceControlRequest 
                    { 
                        DeviceId = light.DeviceId, 
                        Command = "on" 
                    });
                }
                else if (!shouldBeOn && isOn)
                {
                    await controlService.ControlDeviceAsync(new DTOs.DeviceControlRequest 
                    { 
                        DeviceId = light.DeviceId, 
                        Command = "off" 
                    });
                }
            }
        }
    }
}
