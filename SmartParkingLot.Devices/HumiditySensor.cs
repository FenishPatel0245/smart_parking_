using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

/// <summary>
/// Humidity sensor device (non-controllable)
/// </summary>
public class HumiditySensor : BaseDevice
{
    public HumiditySensor()
    {
        Type = DeviceType.HumiditySensor;
        Unit = "%";
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
