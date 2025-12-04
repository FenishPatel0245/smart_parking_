using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Application.DTOs;

// TODO: Implement DTO mappings (e.g., AutoMapper or extension methods)
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class DeviceDto
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsControllable { get; set; }
    public double? CurrentValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double? WarningThreshold { get; set; }
    public double? CriticalThreshold { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

public class TelemetryDto
{
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class AlertDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public double TriggerValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAcknowledged { get; set; }
}

public class EventLogDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? DeviceName { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DeviceControlRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public object? Value { get; set; }
}

public class DeviceControlResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
