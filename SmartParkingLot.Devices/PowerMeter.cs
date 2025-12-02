using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

public class PowerMeter : BaseDevice
{
    public double TotalEnergyConsumed { get; private set; }
    public bool IsMonitoring { get; private set; } = true;

    public PowerMeter()
    {
        Type = DeviceType.PowerMeter;
        Unit = "kW";
        IsControllable = true;
    }

    public override async Task<double> ReadTelemetryAsync()
    {
        if (!IsMonitoring) return 0;

        var (value, newPosition) = await TelemetryFileReader.ReadNextValueAsync(_telemetryFilePath, _currentFilePosition);
        _currentFilePosition = newPosition;
        
        CurrentValue = value;
        // Add to total (kW * hours). Polling every 3s = 3/3600 hours
        TotalEnergyConsumed += value * (3.0 / 3600.0);
        
        UpdateStatus();
        return value;
    }

    public override async Task<bool> ControlAsync(string command, object? value = null)
    {
        switch (command.ToLower())
        {
            case "reset":
                TotalEnergyConsumed = 0;
                return true;

            case "start":
                IsMonitoring = true;
                return true;

            case "stop":
                IsMonitoring = false;
                return true;

            default:
                return false;
        }
    }
}
