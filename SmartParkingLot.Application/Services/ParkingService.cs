using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Application.Services;

public class ParkingService : IParkingService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IDeviceControlService _controlService;
    private readonly IDeviceManagementService _deviceService;

    public ParkingService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IDeviceControlService controlService,
        IDeviceManagementService deviceService)
    {
        _contextFactory = contextFactory;
        _controlService = controlService;
        _deviceService = deviceService;
    }

    public async Task<IEnumerable<ParkingSlot>> GetAllSlotsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ParkingSlots.ToListAsync();
    }

    public async Task<ParkingSlot?> FindBestSlotAsync(string vehicleType)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var slots = await context.ParkingSlots
            .Where(s => !s.IsOccupied && !s.IsReserved)
            .ToListAsync();

        if (!slots.Any()) return null;

        // Smart Logic:
        // 1. If EV, prioritize EV slots
        // 2. If Accessible, prioritize Accessible slots
        // 3. Otherwise, pick closest (simulated by ID order for now)

        if (vehicleType == "Electric")
        {
            var evSlot = slots.FirstOrDefault(s => s.Type == "EV");
            if (evSlot != null) return evSlot;
        }
        else if (vehicleType == "Accessible")
        {
            var accSlot = slots.FirstOrDefault(s => s.Type == "Accessible");
            if (accSlot != null) return accSlot;
        }

        // Default: First available regular slot (or any if no regular left)
        return slots.OrderBy(s => s.Id).FirstOrDefault();
    }

    public async Task<ParkingTicket> CheckInAsync(string plateNumber, string vehicleType)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Re-implement FindBestSlot logic inside here or call helper but need to attach to this context?
        // Better to query this context.
        
        var slots = await context.ParkingSlots
            .Where(s => !s.IsOccupied && !s.IsReserved)
            .ToListAsync();

        ParkingSlot? slot = null;

        if (slots.Any())
        {
             if (vehicleType == "Electric")
                slot = slots.FirstOrDefault(s => s.Type == "EV");
             else if (vehicleType == "Accessible")
                slot = slots.FirstOrDefault(s => s.Type == "Accessible");
             
             if (slot == null)
                slot = slots.OrderBy(s => s.Id).FirstOrDefault();
        }

        if (slot == null)
        {
            throw new InvalidOperationException("No parking slots available.");
        }

        // 1. Assign Slot
        slot.IsOccupied = true;
        slot.CarNumber = plateNumber;
        slot.LastUpdated = DateTime.UtcNow;
        context.ParkingSlots.Update(slot);
        await context.SaveChangesAsync();

        // 2. Trigger Automation (Gate & Lights)
        // Note: _controlService uses its own scope/context, which is fine.
        await TriggerEntryAutomation(slot);

        // 3. Generate Ticket
        return new ParkingTicket
        {
            TicketId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
            PlateNumber = plateNumber,
            SlotNumber = slot.SlotNumber,
            EntryTime = DateTime.UtcNow,
            Status = "Active"
        };
    }

    public async Task<ParkingTicket> CheckOutAsync(string plateNumber)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var slot = await context.ParkingSlots.FirstOrDefaultAsync(s => s.CarNumber == plateNumber);
        if (slot == null)
        {
             throw new InvalidOperationException("Vehicle not found in any slot.");
        }

        // 1. Free Slot
        slot.IsOccupied = false;
        slot.CarNumber = null;
        slot.LastUpdated = DateTime.UtcNow;
        context.ParkingSlots.Update(slot);
        await context.SaveChangesAsync();

        // 2. Trigger Automation (Exit Gate)
        await TriggerExitAutomation();

        return new ParkingTicket
        {
            PlateNumber = plateNumber,
            SlotNumber = slot.SlotNumber,
            ExitTime = DateTime.UtcNow,
            Status = "Completed",
            Fee = 5.00m // Flat rate simulation
            // TODO: Integrate real payment gateway (e.g., Stripe, PayPal)
        };
    }

    private async Task TriggerEntryAutomation(ParkingSlot slot)
    {
        // Open Entry Gate
        // We need to find device ID. _controlService takes ID.
        // We can query devices using our context.
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var gate = await context.Devices.FirstOrDefaultAsync(d => d.Type == Domain.Enums.DeviceType.EntryGateBarrier);
        if (gate != null)
        {
            await _controlService.ControlDeviceAsync(new DeviceControlRequest { DeviceId = gate.DeviceId, Command = "open" });
        }

        // Turn On Lights
        var lights = await context.Devices.Where(d => d.Type == Domain.Enums.DeviceType.SmartLighting).ToListAsync();
        foreach (var light in lights)
        {
            await _controlService.ControlDeviceAsync(new DeviceControlRequest { DeviceId = light.DeviceId, Command = "on" });
        }
    }

    private async Task TriggerExitAutomation()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var gate = await context.Devices.FirstOrDefaultAsync(d => d.Type == Domain.Enums.DeviceType.EntryGateBarrier);
        if (gate != null)
        {
            await _controlService.ControlDeviceAsync(new DeviceControlRequest { DeviceId = gate.DeviceId, Command = "open" });
        }
    }
}
