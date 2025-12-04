using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

// summary
public class EntranceSensor : BaseDevice
{
    public bool CarDetected { get; private set; }
    public string? LastCarNumber { get; private set; }

    public EntranceSensor()
    {
        Type = DeviceType.EntranceSensor;
        Unit = "Bool";
        IsControllable = false;
    }

    public override async Task<double> ReadTelemetryAsync()
    {
        // Simulate random car arrival
        // In a real scenario, this would read from a hardware sensor or camera
        var random = new Random();
        CarDetected = random.NextDouble() > 0.8; // 20% chance of car detection per poll
        
        if (CarDetected)
        {
            LastCarNumber = $"ABC-{random.Next(100, 999)}";
            CurrentValue = 1;
        }
        else
        {
            CurrentValue = 0;
        }
        
        UpdateStatus();
        return CurrentValue;
    }
}
