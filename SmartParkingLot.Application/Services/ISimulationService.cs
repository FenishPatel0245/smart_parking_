using System.Threading.Tasks;

namespace SmartParkingLot.Application.Services
{
    public interface ISimulationService
    {
        // Vehicle Simulations
        Task SimulateCarArrival();
        Task SimulateCarEntry();
        Task SimulateParking(string slotNumber);
        Task SimulateExit(string slotNumber);
        
        // Slot Simulations
        Task SetSlotState(string slotNumber, bool isOccupied);
        Task SetSlotFaulty(string slotNumber);
        Task SetSlotMaintenance(string slotNumber, bool isMaintenance);
        
        // Gate Simulations
        Task ToggleGate();
        Task SimulateGateJam();
        
        // Lighting Simulations
        Task ToggleLights();
        Task SimulateDayNight(bool isNight);
        
        // Alert Simulations
        Task TriggerEmergency(string type);
        Task ResetSystem();
        
        // Telemetry
        Task ReloadTelemetry();
    }
}
