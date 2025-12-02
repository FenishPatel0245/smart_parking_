using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

public interface IAlertRepository : IRepository<Alert>
{
    Task<IEnumerable<Alert>> GetUnacknowledgedAlertsAsync();
    Task<IEnumerable<Alert>> GetAlertsByDeviceIdAsync(int deviceId);
    Task AcknowledgeAlertAsync(int alertId, int userId);
}

public class AlertRepository : Repository<Alert>, IAlertRepository
{
    public AlertRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Alert>> GetUnacknowledgedAlertsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Alerts
            .Include(a => a.Device)
            .Where(a => !a.IsAcknowledged)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsByDeviceIdAsync(int deviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Alerts
            .Where(a => a.DeviceId == deviceId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task AcknowledgeAlertAsync(int alertId, int userId)
    {
        var alert = await GetByIdAsync(alertId);
        if (alert != null && !alert.IsAcknowledged)
        {
            alert.IsAcknowledged = true;
            alert.AcknowledgedAt = DateTime.UtcNow;
            alert.AcknowledgedByUserId = userId;
            await UpdateAsync(alert);
        }
    }
}
