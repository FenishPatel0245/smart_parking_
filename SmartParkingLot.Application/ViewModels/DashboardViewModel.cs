using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Application.Services;
using SmartParkingLot.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace SmartParkingLot.Application.ViewModels;

public class DashboardViewModel
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISystemStateService _systemState;
    
    public List<DeviceDto> Devices { get; private set; } = new();
    public List<AlertDto> ActiveAlerts { get; private set; } = new();
    public List<ParkingSlot> ParkingSlots { get; private set; } = new();

    public int TotalSlots => ParkingSlots.Count;
    public int OccupiedSlots => ParkingSlots.Count(s => s.IsOccupied);
    public int AvailableSlots => ParkingSlots.Count(s => !s.IsOccupied && !s.IsMaintenance);
    public decimal TotalRevenue => 1250.00m; // Simulated for now

    public bool IsAutoModeEnabled 
    { 
        get => _systemState.IsAutoModeEnabled;
        set => _systemState.IsAutoModeEnabled = value;
    }
    public bool IsConnected { get; set; } = false;

    public event Action? OnStateChanged;

    private readonly IActivityLogService _logService;

    public DashboardViewModel(
        IServiceScopeFactory scopeFactory,
        ISystemStateService systemState,
        IActivityLogService logService)
    {
        _scopeFactory = scopeFactory;
        _systemState = systemState;
        _logService = logService;
    }

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    public string? ErrorMessage { get; private set; }

    public async Task LoadDataAsync()
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceManagementService>();
                var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                ErrorMessage = null;
                Devices = (await deviceService.GetAllDevicesAsync()).ToList();
                ActiveAlerts = (await alertService.GetUnacknowledgedAlertsAsync()).ToList();
                ParkingSlots = (await deviceService.GetParkingSlotsAsync()).ToList();
            }
            
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading data: {ex.Message}";
            NotifyStateChanged();
        }
    }

    public async Task ToggleAutoMode(bool isEnabled)
    {
        IsAutoModeEnabled = isEnabled;
        _logService.LogActivity(
            "Auto-Mode Toggled", 
            $"Auto-mode was turned {(isEnabled ? "ON" : "OFF")}.", 
            "Info", 
            "System", 
            "fas fa-robot");
        // State change event from service will trigger UI update if we subscribe, 
        // but for now local notify is enough as getter delegates to service
        NotifyStateChanged();
    }

    public void UpdateDevice(DeviceDto updatedDevice)
    {
        var device = Devices.FirstOrDefault(d => d.Id == updatedDevice.Id);
        if (device != null)
        {
            device.Status = updatedDevice.Status;
            device.CurrentValue = updatedDevice.CurrentValue;
            device.LastUpdatedAt = updatedDevice.LastUpdatedAt;
            NotifyStateChanged();
        }
    }

    public void AddAlert(AlertDto alert)
    {
        if (!ActiveAlerts.Any(a => a.Id == alert.Id))
        {
            ActiveAlerts.Insert(0, alert);
            NotifyStateChanged();
        }
    }

    public async Task HandleSignalRUpdate()
    {
        await LoadDataAsync();
    }

    public void SetConnectionStatus(bool isConnected)
    {
        IsConnected = isConnected;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
