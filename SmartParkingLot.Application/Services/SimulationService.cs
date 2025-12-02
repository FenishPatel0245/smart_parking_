using System;
using System.Linq;
using System.Threading.Tasks;
using SmartParkingLot.Domain.Enums;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Repositories;
using SmartParkingLot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartParkingLot.Application.Services
{
    public class SimulationService : ISimulationService
    {
        private readonly IDeviceManagementService _deviceService;
        private readonly IAlertService _alertService;
        private readonly ISystemStateService _systemState;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IBroadcastService _broadcastService;

        public SimulationService(
            IDeviceManagementService deviceService,
            IAlertService alertService,
            ISystemStateService systemState,
            IEventLogRepository eventLogRepository,
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IBroadcastService broadcastService)
        {
            _deviceService = deviceService;
            _alertService = alertService;
            _systemState = systemState;
            _eventLogRepository = eventLogRepository;
            _contextFactory = contextFactory;
            _broadcastService = broadcastService;
        }

        // Vehicle Simulations
        public async Task SimulateCarArrival()
        {
            await LogEvent("Vehicle", "Simulation: Car arrived at entrance");
            // Trigger entrance sensor
            var sensor = (await _deviceService.GetAllDevicesAsync())
                .FirstOrDefault(d => d.Type == DeviceType.EntranceSensor.ToString());
            
            if (sensor != null)
            {
                // In a real sim we might update a value, here we just log
                await LogEvent("Sensor", $"Entrance Sensor {sensor.DeviceId} triggered");
            }
        }

        public async Task SimulateCarEntry()
        {
            await LogEvent("Vehicle", "Simulation: Car entered parking lot");
            // Open gate
            await ToggleGate();
        }

        public async Task SimulateParking(string slotNumber)
        {
            await SetSlotState(slotNumber, true);
            await LogEvent("Vehicle", $"Simulation: Car parked in {slotNumber}");
        }

        public async Task SimulateExit(string slotNumber)
        {
            await SetSlotState(slotNumber, false);
            await LogEvent("Vehicle", $"Simulation: Car left {slotNumber}");
        }

        // Slot Simulations
        public async Task SetSlotState(string slotNumber, bool isOccupied)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var slot = await context.ParkingSlots.FirstOrDefaultAsync(s => s.SlotNumber == slotNumber);
            if (slot != null)
            {
                slot.IsOccupied = isOccupied;
                slot.LastUpdated = DateTime.UtcNow;
                await context.SaveChangesAsync();
                await LogEvent("Slot", $"Slot {slotNumber} state changed to {(isOccupied ? "Occupied" : "Empty")}");
                await _broadcastService.BroadcastDashboardUpdateAsync();
            }
        }

        public async Task SetSlotFaulty(string slotNumber)
        {
             await LogEvent("Slot", $"Simulation: Slot {slotNumber} sensor reported FAULT");
             // Also set as maintenance/faulty in DB
             await SetSlotMaintenance(slotNumber, true);
             
             await _alertService.CreateAlertAsync(new Alert 
             { 
                 Severity = AlertSeverity.Warning, 
                 Message = $"Slot Sensor {slotNumber} Malfunction",
                 DeviceId = 0 // Placeholder
             });
             await _broadcastService.BroadcastDashboardUpdateAsync();
        }

        public async Task SetSlotMaintenance(string slotNumber, bool isMaintenance)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var slot = await context.ParkingSlots.FirstOrDefaultAsync(s => s.SlotNumber == slotNumber);
            if (slot != null)
            {
                slot.IsMaintenance = isMaintenance;
                slot.LastUpdated = DateTime.UtcNow;
                await context.SaveChangesAsync();
                await LogEvent("Slot", $"Slot {slotNumber} maintenance mode {(isMaintenance ? "ENABLED" : "DISABLED")}");
                await _broadcastService.BroadcastDashboardUpdateAsync();
            }
        }

        // Gate Simulations
        public async Task ToggleGate()
        {
            // Find a gate
             var gate = (await _deviceService.GetControllableDevicesAsync())
                .FirstOrDefault(d => d.Name.Contains("Gate") || d.Type.Contains("Gate"));
            
            if (gate != null)
            {
                // Toggle logic would go here if we had state, for now log
                await LogEvent("Gate", $"Simulation: Gate {gate.Name} toggled");
                await _broadcastService.BroadcastDashboardUpdateAsync();
            }
            else
            {
                await LogEvent("Gate", "Simulation: Gate toggled (Virtual)");
            }
        }

        public async Task SimulateGateJam()
        {
            await _alertService.CreateAlertAsync(new Alert 
            { 
                Severity = AlertSeverity.Critical, 
                Message = "Main Entrance Gate JAMMED",
                DeviceId = 0 
            });
        }

        // Lighting Simulations
        public async Task ToggleLights()
        {
            var lights = (await _deviceService.GetControllableDevicesAsync())
                .Where(d => d.Type == DeviceType.SmartLighting.ToString());

            foreach (var light in lights)
            {
                // Toggle logic
                await LogEvent("Light", $"Simulation: Light {light.Name} toggled");
            }
        }

        public async Task SimulateDayNight(bool isNight)
        {
            await LogEvent("Environment", $"Simulation: Time set to {(isNight ? "NIGHT" : "DAY")}");
            if (isNight && _systemState.IsAutoModeEnabled)
            {
                await LogEvent("Automation", "Auto-Mode: Turning lights ON for Night");
            }
        }

        // Alert Simulations
        public async Task TriggerEmergency(string type)
        {
            await _alertService.CreateAlertAsync(new Alert 
            { 
                Severity = AlertSeverity.Critical, 
                Message = $"SIMULATION: {type} EMERGENCY TRIGGERED",
                DeviceId = 0 
            });
        }

        public async Task ResetSystem()
        {
            // Clear alerts? Reset slots?
            await LogEvent("System", "Simulation: System Reset Triggered");
        }

        public async Task ReloadTelemetry()
        {
             await LogEvent("System", "Simulation: Telemetry Reloaded");
        }

        private async Task LogEvent(string type, string desc)
        {
            await _eventLogRepository.AddAsync(new EventLog 
            { 
                EventType = type, 
                Description = desc, 
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
