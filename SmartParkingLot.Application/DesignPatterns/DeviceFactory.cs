using SmartParkingLot.Devices;
using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Application.DesignPatterns;

public interface IDeviceFactory
{
    BaseDevice CreateDevice(DeviceType type);
}

public class DeviceFactory : IDeviceFactory
{
    public BaseDevice CreateDevice(DeviceType type)
    {
        return type switch
        {
            DeviceType.TemperatureSensor => new TemperatureSensor(),
            DeviceType.HumiditySensor => new HumiditySensor(),
            DeviceType.EntryGateBarrier => new EntryGateBarrier(),
            DeviceType.SmartLighting => new SmartLighting(),
            DeviceType.VentilationFan => new VentilationFan(),
            DeviceType.TrafficCounter => new TrafficCounter(),
            DeviceType.PowerMeter => new PowerMeter(),
            DeviceType.EntranceSensor => new EntranceSensor(),
            DeviceType.SlotSensor => new SlotSensor(),
            _ => throw new ArgumentException($"Unknown device type: {type}")
        };
    }
}
