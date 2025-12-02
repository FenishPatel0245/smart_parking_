namespace SmartParkingLot.Domain.Models;

/// <summary>
/// Telemetry reading from a device
/// </summary>
public class TelemetryReading
{
    public int Id { get; set; }
    
    public int DeviceId { get; set; }
    
    public double Value { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Metadata { get; set; }
    
    // Navigation property
    public Device Device { get; set; } = null!;
}
