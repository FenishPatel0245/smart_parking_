using System;

namespace SmartParkingLot.Domain.Models;

public class ParkingTransaction
{
    public int Id { get; set; }
    public int? ParkingSlotId { get; set; }
    public decimal Amount { get; set; }
    // TODO: Fix DateTime Offset issue
    public DateTime TransactionDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; 
    public string Status { get; set; } = string.Empty; // "Completed", "Pending", "Failed"
    // TODO: Add field for external transaction ID (e.g., Stripe PaymentIntentId)
    
    // Navigation property
    public ParkingSlot? ParkingSlot { get; set; }
}
