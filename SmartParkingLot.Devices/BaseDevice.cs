using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

/// <summary>
/// Base abstract class for all device types
/// </summary>
public abstract class BaseDevice
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeviceType Type { get; protected set; }
    public DeviceStatus Status { get; protected set; } = DeviceStatus.Offline;
    public double CurrentValue { get; protected set; }
    public double? WarningThreshold { get; set; }
    public double? CriticalThreshold { get; set; }
    public string Unit { get; protected set; } = string.Empty;
    public bool IsControllable { get; protected set; }
    public DateTime LastUpdate { get; protected set; }
    
    protected string _telemetryFilePath = string.Empty;
    protected int _currentFilePosition = 0;

    /// <summary>
    /// Read telemetry value from file
    /// </summary>
    public abstract Task<double> ReadTelemetryAsync();

    /// <summary>
    /// Update device status based on current value and thresholds
    /// </summary>
    public virtual void UpdateStatus()
    {
        if (WarningThreshold.HasValue && CriticalThreshold.HasValue)
        {
            if (CurrentValue >= CriticalThreshold.Value)
            {
                Status = DeviceStatus.Critical;
            }
            else if (CurrentValue >= WarningThreshold.Value)
            {
                Status = DeviceStatus.Warning;
            }
            else
            {
                Status = DeviceStatus.Normal;
            }
        }
        else
        {
            Status = DeviceStatus.Normal;
        }
    }

    /// <summary>
    /// Set telemetry file path
    /// </summary>
    public void SetTelemetryFilePath(string filePath)
    {
        _telemetryFilePath = filePath;
    }

    /// <summary>
    /// Control device (for controllable devices)
    /// </summary>
    public virtual Task<bool> ControlAsync(string command, object? value = null)
    {
        if (!IsControllable)
        {
            throw new InvalidOperationException($"Device {Name} is not controllable");
        }
        return Task.FromResult(false);
    }
}
