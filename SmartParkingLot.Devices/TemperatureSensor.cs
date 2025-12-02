using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

/// <summary>
/// Temperature sensor device (non-controllable)
/// </summary>
public class TemperatureSensor : BaseDevice
{
    public TemperatureSensor()
    {
        Type = DeviceType.TemperatureSensor;
        Unit = "°F";
        IsControllable = false;
    }

    public override async Task<double> ReadTelemetryAsync()
    {
        if (string.IsNullOrEmpty(_telemetryFilePath))
        {
            throw new InvalidOperationException("Telemetry file path not set");
        }

        var (value, newPosition) = await TelemetryFileReader.ReadNextValueAsync(_telemetryFilePath, _currentFilePosition);
        _currentFilePosition = newPosition;
        CurrentValue = value;
        LastUpdate = DateTime.UtcNow;
        UpdateStatus();
        
        return CurrentValue;
    }
}
