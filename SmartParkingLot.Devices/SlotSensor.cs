using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

public class SlotSensor : BaseDevice
{
    public string SlotNumber { get; set; } = string.Empty;
    public bool IsOccupied { get; private set; }

    public SlotSensor()
    {
        Type = DeviceType.SlotSensor;
        Unit = "Bool";
        IsControllable = false;
    }


    public override async Task<double> ReadTelemetryAsync()
    {
        // Simulate slot occupancy changes
        // In a real scenario, this would read from an ultrasonic/magnetic sensor
        var random = new Random();
        
        // 10% chance to change state
        if (random.NextDouble() > 0.9)
        {
            IsOccupied = !IsOccupied;
        }
        
        CurrentValue = IsOccupied ? 1 : 0;
        
        UpdateStatus();
        return CurrentValue;
    }
}
