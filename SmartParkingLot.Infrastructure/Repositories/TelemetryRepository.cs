using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

public interface ITelemetryRepository : IRepository<TelemetryReading>
{
    Task<IEnumerable<TelemetryReading>> GetByDeviceIdAsync(int deviceId, int count = 100);
    Task<TelemetryReading?> GetLatestByDeviceIdAsync(int deviceId);
    Task<IEnumerable<TelemetryReading>> GetByDeviceIdAndDateRangeAsync(int deviceId, DateTime start, DateTime end);
}

public class TelemetryRepository : Repository<TelemetryReading>, ITelemetryRepository
{
    public TelemetryRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<TelemetryReading>> GetByDeviceIdAsync(int deviceId, int count = 100)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.TelemetryReadings
            .Where(t => t.DeviceId == deviceId)
            .OrderByDescending(t => t.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<TelemetryReading?> GetLatestByDeviceIdAsync(int deviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.TelemetryReadings
            .Where(t => t.DeviceId == deviceId)
            .OrderByDescending(t => t.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TelemetryReading>> GetByDeviceIdAndDateRangeAsync(int deviceId, DateTime start, DateTime end)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.TelemetryReadings
            .Where(t => t.DeviceId == deviceId && t.Timestamp >= start && t.Timestamp <= end)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();
    }
}
