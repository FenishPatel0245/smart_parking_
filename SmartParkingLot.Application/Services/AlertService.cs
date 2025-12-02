using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Domain.Enums;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace SmartParkingLot.Application.Services;

public interface IAlertService
{
    Task CheckAndGenerateAlertAsync(Device device, double currentValue);
    Task<IEnumerable<AlertDto>> GetUnacknowledgedAlertsAsync();
    Task<IEnumerable<AlertDto>> GetDeviceAlertsAsync(int deviceId);
    Task AcknowledgeAlertAsync(int alertId, int userId);
    Task CreateAlertAsync(Alert alert);
}

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IDeviceSubject _deviceSubject;
    private readonly IAlertStrategy _alertStrategy;
    private readonly ILogger<AlertService> _logger;
    private readonly Dictionary<int, DateTime> _lastAlertTimes = new();

    public AlertService(
        IAlertRepository alertRepository,
        IDeviceSubject deviceSubject,
        IAlertStrategy alertStrategy,
        ILogger<AlertService> logger)
    {
        _alertRepository = alertRepository;
        _deviceSubject = deviceSubject;
        _alertStrategy = alertStrategy;
        _logger = logger;
    }

    public async Task CheckAndGenerateAlertAsync(Device device, double currentValue)
    {
        try
        {
            // Use Strategy pattern to determine if alert should be generated
            if (!_alertStrategy.ShouldGenerateAlert(currentValue, device.WarningThreshold, device.CriticalThreshold))
            {
                return;
            }

            // Throttle alerts (don't generate same alert within 60 seconds)
            if (_lastAlertTimes.TryGetValue(device.Id, out var lastTime))
            {
                if ((DateTime.UtcNow - lastTime).TotalSeconds < 60)
                {
                    return;
                }
            }

            var severity = _alertStrategy.GetSeverity(currentValue, device.WarningThreshold, device.CriticalThreshold);
            var message = _alertStrategy.GenerateMessage(device.Name, currentValue, device.Unit, severity);

            var alert = new Alert
            {
                DeviceId = device.Id,
                Severity = severity,
                Message = message,
                TriggerValue = currentValue,
                CreatedAt = DateTime.UtcNow,
                IsAcknowledged = false
            };

            var createdAlert = await _alertRepository.AddAsync(alert);
            _lastAlertTimes[device.Id] = DateTime.UtcNow;

            // Notify observers
            var alertDto = MapToDto(createdAlert, device.Name);
            await _deviceSubject.NotifyAlert(alertDto);

            _logger.LogWarning($"Alert generated for device {device.Name}: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating alert for device {device.Id}");
        }
    }

    public async Task<IEnumerable<AlertDto>> GetUnacknowledgedAlertsAsync()
    {
        var alerts = await _alertRepository.GetUnacknowledgedAlertsAsync();
        return alerts.Select(a => MapToDto(a, a.Device?.Name ?? "Unknown"));
    }

    public async Task<IEnumerable<AlertDto>> GetDeviceAlertsAsync(int deviceId)
    {
        var alerts = await _alertRepository.GetAlertsByDeviceIdAsync(deviceId);
        return alerts.Select(a => MapToDto(a, a.Device?.Name ?? "Unknown"));
    }

    public async Task AcknowledgeAlertAsync(int alertId, int userId)
    {
        await _alertRepository.AcknowledgeAlertAsync(alertId, userId);
    }

    public async Task CreateAlertAsync(Alert alert)
    {
        alert.CreatedAt = DateTime.UtcNow;
        alert.IsAcknowledged = false;
        
        var createdAlert = await _alertRepository.AddAsync(alert);
        
        // Notify observers
        // We need a device name, but for manual alerts it might be generic
        string deviceName = "System";
        if (alert.DeviceId > 0)
        {
             // Ideally fetch device name, but for now...
             // We can leave it as System or handle it if we had access to repo here easily
        }

        var alertDto = MapToDto(createdAlert, deviceName);
        await _deviceSubject.NotifyAlert(alertDto);
        
        _logger.LogWarning($"Manual Alert generated: {alert.Message}");
    }

    private AlertDto MapToDto(Alert alert, string deviceName)
    {
        return new AlertDto
        {
            Id = alert.Id,
            DeviceId = alert.DeviceId,
            DeviceName = deviceName,
            Severity = alert.Severity.ToString(),
            Message = alert.Message,
            TriggerValue = alert.TriggerValue,
            CreatedAt = alert.CreatedAt,
            IsAcknowledged = alert.IsAcknowledged
        };
    }
}
