using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

// <summary>
/// Generic repository implementation
/// </summary>
// TODO: Refactor implementation to support Unit of Work (remove context creation per method)
// TODO: Enhance generic repository pattern
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public Repository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        // TODO: Update database queries to optimize performance (e.g., use projections, avoid N+1)
        // TODO: Update database queries for optimizations
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
