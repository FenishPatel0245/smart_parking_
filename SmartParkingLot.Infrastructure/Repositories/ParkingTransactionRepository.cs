using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

public interface IParkingTransactionRepository : IRepository<ParkingTransaction>
{
    Task<decimal> GetTotalRevenueAsync();
    Task<IEnumerable<ParkingTransaction>> GetRecentTransactionsAsync(int count);
}

public class ParkingTransactionRepository : Repository<ParkingTransaction>, IParkingTransactionRepository
{
    public ParkingTransactionRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Fix for SQLite decimal Sum issue: Fetch data first, then sum in memory
        var amounts = await context.ParkingTransactions
            .Where(t => t.Status == "Completed")
            .Select(t => t.Amount)
            .ToListAsync();
            
        return amounts.Sum();
    }

    public async Task<IEnumerable<ParkingTransaction>> GetRecentTransactionsAsync(int count)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ParkingTransactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .ToListAsync();
    }
}
