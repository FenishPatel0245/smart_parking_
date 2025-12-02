using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

public interface IEventLogRepository : IRepository<EventLog>
{
    Task<IEnumerable<EventLog>> GetRecentLogsAsync(int count = 100);
    Task<IEnumerable<EventLog>> GetLogsByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<EventLog>> GetLogsByDeviceIdAsync(int deviceId);
}

public class EventLogRepository : Repository<EventLog>, IEventLogRepository
{
    public EventLogRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<EventLog>> GetRecentLogsAsync(int count = 100)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.EventLogs
            .Include(e => e.User)
            .Include(e => e.Device)
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventLog>> GetLogsByDateRangeAsync(DateTime start, DateTime end)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.EventLogs
            .Include(e => e.User)
            .Include(e => e.Device)
            .Where(e => e.Timestamp >= start && e.Timestamp <= end)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventLog>> GetLogsByDeviceIdAsync(int deviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.EventLogs
            .Include(e => e.User)
            .Where(e => e.DeviceId == deviceId)
            .OrderByDescending(e => e.Timestamp)
            .Take(100)
            .ToListAsync();
    }
}
