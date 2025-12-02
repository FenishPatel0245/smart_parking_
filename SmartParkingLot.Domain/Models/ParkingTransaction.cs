using System;

namespace SmartParkingLot.Domain.Models;

public class ParkingTransaction
{
    public int Id { get; set; }
    public int? ParkingSlotId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Completed", "Pending", "Failed"
    
    // Navigation property
    public ParkingSlot? ParkingSlot { get; set; }
}
