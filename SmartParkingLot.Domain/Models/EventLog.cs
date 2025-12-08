namespace SmartParkingLot.Domain.Models;

/// <summary>
// System event log for audit trail
// </summary>
public class EventLog
{
    public int Id { get; set; }
    
    public string EventType { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public int? UserId { get; set; }
    
    public int? DeviceId { get; set; }
    
    public string? AdditionalData { get; set; }
    
    // TODO: Consider using DateTimeOffset for better timezone handling
    // TODO: Use DateTimeOffset for event logs
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User? User { get; set; }
    public Device? Device { get; set; }
}
