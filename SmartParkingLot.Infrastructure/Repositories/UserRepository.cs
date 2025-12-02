using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Data;

namespace SmartParkingLot.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }
}
