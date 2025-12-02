using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Domain.Models;

/// <summary>
/// Alert generated when device thresholds are violated
/// </summary>
public class Alert
{
    public int Id { get; set; }
    
    public int DeviceId { get; set; }
    
    public AlertSeverity Severity { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public double TriggerValue { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsAcknowledged { get; set; } = false;
    
    public DateTime? AcknowledgedAt { get; set; }
    
    public int? AcknowledgedByUserId { get; set; }
    
    // Navigation properties
    public Device Device { get; set; } = null!;
    public User? AcknowledgedByUser { get; set; }
}
