using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Application.Services;

public interface IParkingService
{
    Task<ParkingTicket> CheckInAsync(string plateNumber, string vehicleType);
    Task<ParkingTicket> CheckOutAsync(string plateNumber);
    Task<ParkingSlot?> FindBestSlotAsync(string vehicleType);
    Task<IEnumerable<ParkingSlot>> GetAllSlotsAsync();
}

public class ParkingTicket
{
    public string TicketId { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string SlotNumber { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public decimal Fee { get; set; }
    public string Status { get; set; } = "Active"; // Active, Paid, Completed
}
