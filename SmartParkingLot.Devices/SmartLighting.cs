using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Devices;

public class SmartLighting : BaseDevice
{
    public bool IsOn { get; private set; }
    public double BrightnessLevel { get; private set; } // 0-100%


    // Constructor
    public SmartLighting()
    {
        Type = DeviceType.SmartLighting;
        Unit = "Lux"; // Sensor reads ambient light (Lux), but control is Brightness (%)
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
            case "on":
                IsOn = true;
                BrightnessLevel = 100;
                return true;

            case "off":
                IsOn = false;
                BrightnessLevel = 0;
                return true;

            case "dim":
                if (value != null && double.TryParse(value.ToString(), out double level))
                {
                    if (level >= 0 && level <= 100)
                    {
                        BrightnessLevel = level;
                        IsOn = level > 0;
                        return true;
                    }
                }
                return false;

            default:
                return false;
        }
    }
}
