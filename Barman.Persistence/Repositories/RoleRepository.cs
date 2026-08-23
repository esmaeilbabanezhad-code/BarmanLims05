using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<Role>> GetActiveAsync()
    {
        return await _context.Roles
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<Role>> GetDeletedAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Role?> GetDeletedByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsDeleted);
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void Update(Role role)
    {
        _context.Roles.Update(role);
    }

    public void Delete(Role role)
    {
        role.IsDeleted = true;
        role.IsActive = false;

        _context.Roles.Update(role);
    }
}
