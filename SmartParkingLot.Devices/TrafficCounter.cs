using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

public class TrafficCounter : BaseDevice
{
    public double TotalCars { get; private set; }
    public bool IsCalibrated { get; private set; } = true;

    public TrafficCounter()
    {
        Type = DeviceType.TrafficCounter;
        Unit = "Cars/Min";
        IsControllable = true;
    }

    public override async Task<double> ReadTelemetryAsync()
    {
        // Reads cars per minute
        var (value, newPosition) = await TelemetryFileReader.ReadNextValueAsync(_telemetryFilePath, _currentFilePosition);
        _currentFilePosition = newPosition;
        
        CurrentValue = value;
        TotalCars += (value / 60.0) * 3.0; // Approx cars added in 3 seconds polling interval
        
        UpdateStatus();
        return value;
    }

    public override async Task<bool> ControlAsync(string command, object? value = null)
    {
        switch (command.ToLower())
        {
            case "reset":
                TotalCars = 0;
                return true;

            case "calibrate":
                IsCalibrated = true;
                return true;

            default:
                return false;
        }
    }
}
