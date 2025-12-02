namespace SmartParkingLot.Domain.Enums;

/// <summary>
/// Current operational status of a device
/// </summary>
public enum DeviceStatus
{
    /// <summary>
    /// Device is offline or disconnected (gray)
    /// </summary>
    Offline = 0,
    
    /// <summary>
    /// Device is operating normally (green)
    /// </summary>
    Normal = 1,
    
    /// <summary>
    /// Device has warning condition (yellow)
    /// </summary>
    Warning = 2,
    
    /// <summary>
    /// Device has critical condition (red)
    /// </summary>
    Critical = 3
}
