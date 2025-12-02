using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

public class VentilationFan : BaseDevice
{
    public bool IsRunning { get; private set; }
    public double FanSpeed { get; private set; } // RPM

    public VentilationFan()
    {
        Type = DeviceType.VentilationFan;
        Unit = "RPM";
        IsControllable = true;
    }

    public override async Task<double> ReadTelemetryAsync()
    {
        var (value, newPosition) = await TelemetryFileReader.ReadNextValueAsync(_telemetryFilePath, _currentFilePosition);
        _currentFilePosition = newPosition;
        
        CurrentValue = value;
        UpdateStatus();
        return value;
    }

    public override async Task<bool> ControlAsync(string command, object? value = null)
    {
        switch (command.ToLower())
        {
            case "start":
                IsRunning = true;
                FanSpeed = 1200; // Default start speed
                CurrentValue = FanSpeed;
                return true;

            case "stop":
                IsRunning = false;
                FanSpeed = 0;
                CurrentValue = 0;
                return true;

            case "setspeed":
                if (value != null && double.TryParse(value.ToString(), out double speed))
                {
                    if (speed >= 0 && speed <= 3000) // Max 3000 RPM
                    {
                        FanSpeed = speed;
                        IsRunning = speed > 0;
                        CurrentValue = speed;
                        return true;
                    }
                }
                return false;

            default:
                return false;
        }
    }
}
