using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

public interface IDeviceRepository : IRepository<Device>
{
    Task<Device?> GetByDeviceIdAsync(string deviceId);
    Task<IEnumerable<Device>> GetActiveDevicesAsync();
    Task<IEnumerable<Device>> GetControllableDevicesAsync();
}

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<Device?> GetByDeviceIdAsync(string deviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Devices
            .Include(d => d.TelemetryReadings.OrderByDescending(t => t.Timestamp).Take(10))
            .Include(d => d.Alerts.Where(a => !a.IsAcknowledged))
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
    }

    public async Task<IEnumerable<Device>> GetActiveDevicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Devices
            .Where(d => d.IsActive)
            .Include(d => d.TelemetryReadings.OrderByDescending(t => t.Timestamp).Take(1))
            .ToListAsync();
    }

    public async Task<IEnumerable<Device>> GetControllableDevicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Devices
            .Where(d => d.IsActive && d.IsControllable)
            .ToListAsync();
    }
}
