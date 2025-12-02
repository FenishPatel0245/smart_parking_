using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Domain.Models;

/// <summary>
/// Device entity representing a monitoring/control device
/// </summary>
public class Device
{
    public int Id { get; set; }
    
    public string DeviceId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public DeviceType Type { get; set; }
    
    public string Location { get; set; } = string.Empty;
    
    public DeviceStatus Status { get; set; } = DeviceStatus.Offline;
    
    public bool IsControllable { get; set; }
    
    public string TelemetryFilePath { get; set; } = string.Empty;
    
    public double? WarningThreshold { get; set; }
    
    public double? CriticalThreshold { get; set; }
    
    public string Unit { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastUpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<TelemetryReading> TelemetryReadings { get; set; } = new List<TelemetryReading>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
