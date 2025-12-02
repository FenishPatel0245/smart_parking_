namespace SmartParkingLot.Application.Services;

public interface ISystemStateService
{
    bool IsAutoModeEnabled { get; set; }
    event Action? OnStateChanged;
}

public class SystemStateService : ISystemStateService
{
    private bool _isAutoModeEnabled;
    public bool IsAutoModeEnabled
    {
        get => _isAutoModeEnabled;
        set
        {
            if (_isAutoModeEnabled != value)
            {
                _isAutoModeEnabled = value;
                OnStateChanged?.Invoke();
            }
        }
    }

    public event Action? OnStateChanged;
}
