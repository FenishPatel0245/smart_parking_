using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Devices;
using SmartParkingLot.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace SmartParkingLot.Application.Services;

public interface IDeviceControlService
{
    Task<DeviceControlResponse> ControlDeviceAsync(DeviceControlRequest request);
}

public class DeviceControlService : IDeviceControlService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IEventLogRepository _eventLogRepository;
    private readonly IDeviceFactory _deviceFactory;
    private readonly ILogger<DeviceControlService> _logger;
    private readonly Dictionary<int, BaseDevice> _deviceInstances = new();

    public DeviceControlService(
        IDeviceRepository deviceRepository,
        IEventLogRepository eventLogRepository,
        IDeviceFactory deviceFactory,
        ILogger<DeviceControlService> logger)
    {
        _deviceRepository = deviceRepository;
        _eventLogRepository = eventLogRepository;
        _deviceFactory = deviceFactory;
        _logger = logger;
    }

    public async Task<DeviceControlResponse> ControlDeviceAsync(DeviceControlRequest request)
    {
        try
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(request.DeviceId);
            if (device == null)
            {
                return new DeviceControlResponse
                {
                    Success = false,
                    Message = $"Device {request.DeviceId} not found"
                };
            }

            if (!device.IsControllable)
            {
                return new DeviceControlResponse
                {
                    Success = false,
                    Message = $"Device {device.Name} is not controllable"
                };
            }

            // Get or create device instance
            if (!_deviceInstances.TryGetValue(device.Id, out var deviceInstance))
            {
                deviceInstance = _deviceFactory.CreateDevice(device.Type);
                deviceInstance.DeviceId = device.Id;
                deviceInstance.Name = device.Name;
                _deviceInstances[device.Id] = deviceInstance;
            }

            // Execute control command
            var success = await deviceInstance.ControlAsync(request.Command, request.Value);

            if (success)
            {
                await LogControlEventAsync(device.Id, device.Name, request.Command, request.Value);

                return new DeviceControlResponse
                {
                    Success = true,
                    Message = $"Command '{request.Command}' executed successfully on {device.Name}"
                };
            }
            else
            {
                return new DeviceControlResponse
                {
                    Success = false,
                    Message = $"Failed to execute command '{request.Command}' on {device.Name}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error controlling device {request.DeviceId}");
            return new DeviceControlResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    private async Task LogControlEventAsync(int deviceId, string deviceName, string command, object? value)
    {
        var description = value != null
            ? $"Control command '{command}' with value '{value}' executed on device {deviceName}"
            : $"Control command '{command}' executed on device {deviceName}";

        var log = new Domain.Models.EventLog
        {
            EventType = "DeviceControl",
            Description = description,
            DeviceId = deviceId,
            Timestamp = DateTime.UtcNow
        };

        await _eventLogRepository.AddAsync(log);
    }
}
