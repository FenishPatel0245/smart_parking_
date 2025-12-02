using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Domain.Enums;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Application.Services;

public interface IDeviceManagementService
{
    Task<IEnumerable<DeviceDto>> GetAllDevicesAsync();
    Task<DeviceDto?> GetDeviceByIdAsync(int id);
    Task<DeviceDto?> GetDeviceByDeviceIdAsync(string deviceId);
    Task<DeviceDto> CreateDeviceAsync(DeviceDto deviceDto);
    Task UpdateDeviceAsync(DeviceDto deviceDto);
    Task DeleteDeviceAsync(int id);
    Task<IEnumerable<DeviceDto>> GetControllableDevicesAsync();
    Task<IEnumerable<ParkingSlot>> GetParkingSlotsAsync();
}

public class DeviceManagementService : IDeviceManagementService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IEventLogRepository _eventLogRepository;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DeviceManagementService(
        IDeviceRepository deviceRepository,
        IEventLogRepository eventLogRepository,
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _deviceRepository = deviceRepository;
        _eventLogRepository = eventLogRepository;
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllDevicesAsync()
    {
        var devices = await _deviceRepository.GetActiveDevicesAsync();
        return devices.Select(MapToDto);
    }

    public async Task<DeviceDto?> GetDeviceByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        return device != null ? MapToDto(device) : null;
    }

    public async Task<DeviceDto?> GetDeviceByDeviceIdAsync(string deviceId)
    {
        var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
        return device != null ? MapToDto(device) : null;
    }

    public async Task<DeviceDto> CreateDeviceAsync(DeviceDto deviceDto)
    {
        var device = new Device
        {
            DeviceId = deviceDto.DeviceId,
            Name = deviceDto.Name,
            Type = Enum.Parse<DeviceType>(deviceDto.Type),
            Location = deviceDto.Location,
            IsControllable = deviceDto.IsControllable,
            WarningThreshold = deviceDto.WarningThreshold,
            CriticalThreshold = deviceDto.CriticalThreshold,
            Unit = deviceDto.Unit,
            Status = DeviceStatus.Offline,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _deviceRepository.AddAsync(device);
        
        await LogEventAsync("DeviceCreated", $"Device {device.Name} ({device.DeviceId}) created", deviceId: created.Id);

        return MapToDto(created);
    }

    public async Task UpdateDeviceAsync(DeviceDto deviceDto)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceDto.Id);
        if (device == null)
        {
            throw new InvalidOperationException($"Device with ID {deviceDto.Id} not found");
        }

        device.Name = deviceDto.Name;
        device.Location = deviceDto.Location;
        device.WarningThreshold = deviceDto.WarningThreshold;
        device.CriticalThreshold = deviceDto.CriticalThreshold;
        device.LastUpdatedAt = DateTime.UtcNow;

        await _deviceRepository.UpdateAsync(device);
        
        await LogEventAsync("DeviceUpdated", $"Device {device.Name} ({device.DeviceId}) updated", deviceId: device.Id);
    }

    public async Task DeleteDeviceAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device != null)
        {
            await LogEventAsync("DeviceDeleted", $"Device {device.Name} ({device.DeviceId}) deleted", deviceId: id);
            await _deviceRepository.DeleteAsync(id);
        }
    }

    public async Task<IEnumerable<DeviceDto>> GetControllableDevicesAsync()
    {
        var devices = await _deviceRepository.GetControllableDevicesAsync();
        return devices.Select(MapToDto);
    }

    public async Task<IEnumerable<ParkingSlot>> GetParkingSlotsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ParkingSlots.ToListAsync();
    }

    private DeviceDto MapToDto(Device device)
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
            Unit = device.Unit,
            WarningThreshold = device.WarningThreshold,
            CriticalThreshold = device.CriticalThreshold,
            LastUpdatedAt = device.LastUpdatedAt
        };
    }

    private async Task LogEventAsync(string eventType, string description, int? userId = null, int? deviceId = null)
    {
        var log = new EventLog
        {
            EventType = eventType,
            Description = description,
            UserId = userId,
            DeviceId = deviceId,
            Timestamp = DateTime.UtcNow
        };

        await _eventLogRepository.AddAsync(log);
    }
}
