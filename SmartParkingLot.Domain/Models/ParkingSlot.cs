namespace SmartParkingLot.Domain.Models;

public class ParkingSlot
{
    public int Id { get; set; }
    public string SlotNumber { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
    public string? CarNumber { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    // New properties for Smart Parking
    public string Type { get; set; } = "Regular"; // Regular, EV, Accessible
    public bool IsReserved { get; set; } = false;
    public bool IsMaintenance { get; set; } = false;
}
