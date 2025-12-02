using SmartParkingLot.Application.DTOs;

namespace SmartParkingLot.Application.DesignPatterns;

/// <summary>
/// Observer pattern for real-time telemetry updates
/// </summary>
public interface IDeviceObserver
{
    Task OnTelemetryUpdated(TelemetryDto telemetry);
    Task OnDeviceStatusChanged(DeviceDto device);
    Task OnAlertGenerated(AlertDto alert);
}

public interface IDeviceSubject
{
    void Attach(IDeviceObserver observer);
    void Detach(IDeviceObserver observer);
    Task NotifyTelemetryUpdate(TelemetryDto telemetry);
    Task NotifyStatusChange(DeviceDto device);
    Task NotifyAlert(AlertDto alert);
}

public class DeviceSubject : IDeviceSubject
{
    private readonly List<IDeviceObserver> _observers = new();
    private readonly object _lock = new();

    public void Attach(IDeviceObserver observer)
    {
        lock (_lock)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
    }

    public void Detach(IDeviceObserver observer)
    {
        lock (_lock)
        {
            _observers.Remove(observer);
        }
    }

    public async Task NotifyTelemetryUpdate(TelemetryDto telemetry)
    {
        List<IDeviceObserver> observersCopy;
        lock (_lock)
        {
            observersCopy = new List<IDeviceObserver>(_observers);
        }

        foreach (var observer in observersCopy)
        {
            await observer.OnTelemetryUpdated(telemetry);
        }
    }

    public async Task NotifyStatusChange(DeviceDto device)
    {
        List<IDeviceObserver> observersCopy;
        lock (_lock)
        {
            observersCopy = new List<IDeviceObserver>(_observers);
        }

        foreach (var observer in observersCopy)
        {
            await observer.OnDeviceStatusChanged(device);
        }
    }

    public async Task NotifyAlert(AlertDto alert)
    {
        List<IDeviceObserver> observersCopy;
        lock (_lock)
        {
            observersCopy = new List<IDeviceObserver>(_observers);
        }

        foreach (var observer in observersCopy)
        {
            await observer.OnAlertGenerated(alert);
        }
    }
}
