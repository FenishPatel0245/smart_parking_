namespace SmartParkingLot.Domain.Enums;

/// <summary>
/// Alert severity levels for threshold violations
/// </summary>
public enum AlertSeverity
{
    /// <summary>
    /// Informational message
    /// </summary>
    Info = 1,
    
    /// <summary>
    /// Warning threshold exceeded (yellow)
    /// </summary>
    Warning = 2,
    
    /// <summary>
    /// Critical threshold exceeded (red)
    /// </summary>
    Critical = 3
}
