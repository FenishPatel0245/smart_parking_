using System;

namespace SmartParkingLot.Domain.Models;

public class ActivityLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    // TODO: Update datetime offset issue
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionType { get; set; } = "System"; // Gate , Light, Slot, Emergency, System
    public string Severity { get; set; } = "Info"; // Info, Success, Warning, Error
    public string Icon { get; set; } = "fas fa-info-circle";
    public string TriggeredBy { get; set; } = "System";
}
