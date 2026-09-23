using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly ApplicationDbContext _context;

    public UserAccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetByUsernameAsync(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var normalizedUsername = username.Trim();

        return await _context.UserAccounts
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.Username == normalizedUsername &&
                !x.IsDeleted);
    }
    public async Task<List<UserAccount>> GetAllAsync()
    {
        return await _context.UserAccounts
            .Include(x => x.Employee)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Username)
            .ToListAsync();
    }
    public async Task<UserAccount?> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        if (employeeId == Guid.Empty)
            return null;

        return await _context.UserAccounts
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == employeeId &&
                !x.IsDeleted);
    }

    public async Task<UserAccount?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.UserAccounts
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<bool> ExistsByUsernameAsync(
        string username,
        Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var normalizedUsername = username.Trim();

        return await _context.UserAccounts
            .AnyAsync(x =>
                x.Username == normalizedUsername &&
                !x.IsDeleted &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task AddAsync(UserAccount account)
    {
        await _context.UserAccounts.AddAsync(account);
    }

    public void Update(UserAccount account)
    {
        _context.UserAccounts.Update(account);
    }
}