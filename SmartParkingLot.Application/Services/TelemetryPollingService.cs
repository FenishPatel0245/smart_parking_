using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Devices;
using SmartParkingLot.Infrastructure.Repositories;
using SmartParkingLot.Domain.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SmartParkingLot.Application.Services;

/// <summary>
/// Background service that polls telemetry files and generates alerts
/// </summary>
public class TelemetryPollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelemetryPollingService> _logger;
    private readonly Dictionary<int, BaseDevice> _deviceInstances = new();
    private readonly IDeviceFactory _deviceFactory;

    public TelemetryPollingService(
        IServiceProvider serviceProvider,
        IDeviceFactory deviceFactory,
        ILogger<TelemetryPollingService> logger)
    {
        _serviceProvider = serviceProvider;
        _deviceFactory = deviceFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telemetry Polling Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollAllDevicesAsync();
                
                var config = ConfigurationManager.Instance;
                await Task.Delay(TimeSpan.FromSeconds(config.TelemetryPollingIntervalSeconds), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in telemetry polling service");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Telemetry Polling Service stopped");
    }

    private async Task PollAllDevicesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
        var telemetryService = scope.ServiceProvider.GetRequiredService<ITelemetryService>();
        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

        var devices = await deviceRepository.GetActiveDevicesAsync();

        foreach (var device in devices)
        {
            try
            {
                // Get or create device instance
                if (!_deviceInstances.TryGetValue(device.Id, out var deviceInstance))
                {
                    deviceInstance = _deviceFactory.CreateDevice(device.Type);
                    deviceInstance.DeviceId = device.Id;
                    deviceInstance.Name = device.Name;
                    deviceInstance.WarningThreshold = device.WarningThreshold;
                    deviceInstance.CriticalThreshold = device.CriticalThreshold;
                    
                    // Set telemetry file path
                    var config = ConfigurationManager.Instance;
                    var filePath = Path.Combine(config.TelemetryDataPath, device.TelemetryFilePath);
                    deviceInstance.SetTelemetryFilePath(filePath);
                    
                    _deviceInstances[device.Id] = deviceInstance;
                }

                // Read telemetry from file
                if (!string.IsNullOrEmpty(device.TelemetryFilePath))
                {
                    var value = await deviceInstance.ReadTelemetryAsync();
                    await telemetryService.ProcessTelemetryAsync(device.Id, value);
                    
                    // Check for alerts after processing telemetry
                    await alertService.CheckAndGenerateAlertAsync(device, value);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning($"Telemetry file not found for device {device.Name}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error polling device {device.Name} (ID: {device.Id})");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telemetry Polling Service is stopping");
        return base.StopAsync(cancellationToken);
    }
}
