using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

public class EntryGateBarrier : BaseDevice
{
    public double BarrierPosition { get; private set; } // 0 = Closed, 100 = Open
    public bool IsOpen => BarrierPosition >= 90;

    public EntryGateBarrier()
    {
        Type = DeviceType.EntryGateBarrier;
        Unit = "%"; // Position percentage
        IsControllable = true;
    }

    public override async Task<double> ReadTelemetryAsync()
    {
        // Simulate reading barrier position sensor
        // In a real scenario, this would read from a hardware sensor
        var (value, newPosition) = await TelemetryFileReader.ReadNextValueAsync(_telemetryFilePath, _currentFilePosition);
        _currentFilePosition = newPosition;
        
        // If we are controlling the device, the telemetry might just confirm our control action
        // But for simulation, we'll let the file drive the "sensor" reading unless overridden
        
        CurrentValue = value;
        UpdateStatus();
        return value;
    }

    public override async Task<bool> ControlAsync(string command, object? value = null)
    {
        switch (command.ToLower())
        {
            case "open":
                BarrierPosition = 100;
                CurrentValue = 100;
                return true;

            case "close":
                BarrierPosition = 0;
                CurrentValue = 0;
                return true;

            case "setposition":
                if (value != null && double.TryParse(value.ToString(), out double pos))
                {
                    if (pos >= 0 && pos <= 100)
                    {
                        BarrierPosition = pos;
                        CurrentValue = pos;
                        return true;
                    }
                }
                return false;

            default:
                return false;
        }
    }
}
