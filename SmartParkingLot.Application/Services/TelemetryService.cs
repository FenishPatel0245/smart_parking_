using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Devices;
using SmartParkingLot.Domain.Enums;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace SmartParkingLot.Application.Services;

public interface ITelemetryService
{
    Task ProcessTelemetryAsync(int deviceId, double value);
    Task<IEnumerable<TelemetryDto>> GetDeviceTelemetryAsync(int deviceId, int count = 100);
    Task<TelemetryDto?> GetLatestTelemetryAsync(int deviceId);
}

public class TelemetryService : ITelemetryService
{
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDeviceSubject _deviceSubject;
    private readonly ILogger<TelemetryService> _logger;

    public TelemetryService(
        ITelemetryRepository telemetryRepository,
        IDeviceRepository deviceRepository,
        IDeviceSubject deviceSubject,
        ILogger<TelemetryService> logger)
    {
        _telemetryRepository = telemetryRepository;
        _deviceRepository = deviceRepository;
        _deviceSubject = deviceSubject;
        _logger = logger;
    }

    public async Task ProcessTelemetryAsync(int deviceId, double value)
    {
        try
        {
            var device = await _deviceRepository.GetByIdAsync(deviceId);
            if (device == null)
            {
                _logger.LogWarning($"Device {deviceId} not found");
                return;
            }

            // Save telemetry reading
            var reading = new TelemetryReading
            {
                DeviceId = deviceId,
                Value = value,
                Timestamp = DateTime.UtcNow
            };

            await _telemetryRepository.AddAsync(reading);

            // Update device status
            var previousStatus = device.Status;
            device.Status = DetermineDeviceStatus(value, device.WarningThreshold, device.CriticalThreshold);
            device.LastUpdatedAt = DateTime.UtcNow;
            await _deviceRepository.UpdateAsync(device);

            // Notify observers (Observer pattern)
            var telemetryDto = new TelemetryDto
            {
                DeviceId = deviceId,
                DeviceName = device.Name,
                Value = value,
                Unit = device.Unit,
                Timestamp = reading.Timestamp
            };

            await _deviceSubject.NotifyTelemetryUpdate(telemetryDto);

            // Check if status changed
            if (previousStatus != device.Status)
            {
                var deviceDto = MapDeviceToDto(device, value);
                await _deviceSubject.NotifyStatusChange(deviceDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing telemetry for device {deviceId}");
        }
    }

    public async Task<IEnumerable<TelemetryDto>> GetDeviceTelemetryAsync(int deviceId, int count = 100)
    {
        var readings = await _telemetryRepository.GetByDeviceIdAsync(deviceId, count);
        var device = await _deviceRepository.GetByIdAsync(deviceId);

        return readings.Select(r => new TelemetryDto
        {
            DeviceId = r.DeviceId,
            DeviceName = device?.Name ?? "Unknown",
            Value = r.Value,
            Unit = device?.Unit ?? "",
            Timestamp = r.Timestamp
        });
    }

    public async Task<TelemetryDto?> GetLatestTelemetryAsync(int deviceId)
    {
        var reading = await _telemetryRepository.GetLatestByDeviceIdAsync(deviceId);
        if (reading == null) return null;

        var device = await _deviceRepository.GetByIdAsync(deviceId);

        return new TelemetryDto
        {
            DeviceId = reading.DeviceId,
            DeviceName = device?.Name ?? "Unknown",
            Value = reading.Value,
            Unit = device?.Unit ?? "",
            Timestamp = reading.Timestamp
        };
    }

    private DeviceStatus DetermineDeviceStatus(double value, double? warningThreshold, double? criticalThreshold)
    {
        if (criticalThreshold.HasValue && value >= criticalThreshold.Value)
        {
            return DeviceStatus.Critical;
        }

        if (warningThreshold.HasValue && value >= warningThreshold.Value)
        {
            return DeviceStatus.Warning;
        }

        return DeviceStatus.Normal;
    }

    private DeviceDto MapDeviceToDto(Device device, double currentValue)
    {
        return new DeviceDto
        {
            Id = device.Id,
            DeviceId = device.DeviceId,
            Name = device.Name,
            Type = device.Type.ToString(),
            Location = device.Location,
            Status = device.Status.ToString(),
            IsControllable = device.IsControllable,
            CurrentValue = currentValue,
            Unit = device.Unit,
            WarningThreshold = device.WarningThreshold,
            CriticalThreshold = device.CriticalThreshold,
            LastUpdatedAt = device.LastUpdatedAt
        };
    }
}
