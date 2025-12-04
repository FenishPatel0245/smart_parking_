using System;
using System.Threading.Tasks;
using SmartParkingLot.Application.Services;


namespace SmartParkingLot.Application.ViewModels
{
    public class SimulationViewModel
    {
        private readonly ISimulationService _simulationService;
        private readonly IActivityLogService _logService;

        public string StatusMessage { get; private set; } = "Ready";
        public bool IsBusy { get; private set; }

        public SimulationViewModel(
            ISimulationService simulationService,
            IActivityLogService logService)
        {
            _simulationService = simulationService;
            _logService = logService;
        }

        // Vehicle Commands
        public async Task SimulateCarArrival()
        {
            await Execute("Car Arrived", async () => 
            {
                await _simulationService.SimulateCarArrival();
                _logService.LogActivity("Car Arrival", "Simulated car arrival at entry gate.", "Info", "Vehicle", "fas fa-car");
            });
        }

        public async Task SimulateCarEntry()
        {
            await Execute("Car Entered", async () => 
            {
                await _simulationService.SimulateCarEntry();
                _logService.LogActivity("Car Entry", "Car entered the parking lot.", "Success", "Vehicle", "fas fa-arrow-right");
            });
        }

        public async Task SimulateParking(string slot)
        {
            await Execute($"Parked in {slot}", async () => 
            {
                await _simulationService.SimulateParking(slot);
                _logService.LogActivity("Car Parked", $"Car parked in slot {slot}.", "Success", "Slot", "fas fa-parking");
            });
        }

        public async Task SimulateExit(string slot)
        {
            await Execute($"Exited {slot}", async () => 
            {
                await _simulationService.SimulateExit(slot);
                _logService.LogActivity("Car Exited", $"Car exited from slot {slot}.", "Info", "Slot", "fas fa-sign-out-alt");
            });
        }

        // Slot Commands
        public async Task SetSlotOccupied(string slot)
        {
            await Execute($"Slot {slot} Occupied", async () => 
            {
                await _simulationService.SetSlotState(slot, true);
                _logService.LogActivity("Slot Occupied", $"Slot {slot} marked as occupied.", "Info", "Slot", "fas fa-car-side");
            });
        }

        public async Task SetSlotEmpty(string slot)
        {
            await Execute($"Slot {slot} Empty", async () => 
            {
                await _simulationService.SetSlotState(slot, false);
                _logService.LogActivity("Slot Freed", $"Slot {slot} marked as empty.", "Info", "Slot", "fas fa-check");
            });
        }

        public async Task SetSlotFaulty(string slot)
        {
            await Execute($"Slot {slot} Faulty", async () => 
            {
                await _simulationService.SetSlotFaulty(slot);
                _logService.LogActivity("Slot Fault", $"Slot {slot} reported a fault.", "Error", "Slot", "fas fa-exclamation-triangle");
            });
        }

        public async Task SetSlotMaintenance(string slot, bool isMaintenance)
        {
            await Execute($"Slot {slot} Maintenance {(isMaintenance ? "ON" : "OFF")}", async () => 
            {
                await _simulationService.SetSlotMaintenance(slot, isMaintenance);
                _logService.LogActivity("Maintenance Update", $"Slot {slot} maintenance {(isMaintenance ? "started" : "ended")}.", "Warning", "Slot", "fas fa-tools");
            });
        }

        // Gate Commands
        public async Task ToggleGate()
        {
            await Execute("Gate Toggled", async () => 
            {
                await _simulationService.ToggleGate();
                _logService.LogActivity("Gate Toggled", "Entry/Exit gate toggled.", "Info", "Gate", "fas fa-torii-gate");
            });
        }

        public async Task JamGate()
        {
            await Execute("Gate Jammed", async () => 
            {
                await _simulationService.SimulateGateJam();
                _logService.LogActivity("Gate Jammed", "Gate malfunction simulated.", "Error", "Gate", "fas fa-times-circle");
            });
        }

        // Light Commands
        public async Task ToggleLights()
        {
            await Execute("Lights Toggled", async () => 
            {
                await _simulationService.ToggleLights();
                _logService.LogActivity("Lights Toggled", "Parking lot lights toggled.", "Info", "Light", "fas fa-lightbulb");
            });
        }

        public async Task SetNightMode()
        {
            await Execute("Night Mode ON", async () => 
            {
                await _simulationService.SimulateDayNight(true);
                _logService.LogActivity("Night Mode", "Environment set to Night mode.", "Info", "System", "fas fa-moon");
            });
        }

        public async Task SetDayMode()
        {
            await Execute("Day Mode ON", async () => 
            {
                await _simulationService.SimulateDayNight(false);
                _logService.LogActivity("Day Mode", "Environment set to Day mode.", "Info", "System", "fas fa-sun");
            });
        }

        // Alert Commands
        public async Task TriggerFireAlarm()
        {
            await Execute("FIRE ALARM TRIGGERED", async () => 
            {
                await _simulationService.TriggerEmergency("Fire");
                _logService.LogActivity("FIRE ALARM", "Fire emergency triggered manually.", "Critical", "Emergency", "fas fa-fire");
            });
        }

        public async Task TriggerSecurityBreach()
        {
            await Execute("SECURITY BREACH TRIGGERED", async () => 
            {
                await _simulationService.TriggerEmergency("Security");
                _logService.LogActivity("Security Breach", "Security breach triggered manually.", "Critical", "Emergency", "fas fa-user-secret");
            });
        }

        public async Task ResetSystem()
        {
            await Execute("System Reset", async () => 
            {
                await _simulationService.ResetSystem();
                _logService.LogActivity("System Reset", "System reset initiated.", "Warning", "System", "fas fa-redo");
            });
        }

        // Helper
        private async Task Execute(string successMessage, Func<Task> action)
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Executing...";
            try
            {
                await action();
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
