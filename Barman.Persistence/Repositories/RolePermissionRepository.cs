using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ApplicationDbContext _context;

    public RolePermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RolePermission rolePermission)
    {
        await _context.RolePermissions.AddAsync(rolePermission);
    }

    public async Task<List<RolePermission>> GetByRoleIdAsync(
        Guid roleId)
    {
        return await _context.RolePermissions
            .Include(x => x.Permission)
            .Where(x => x.RoleId == roleId)
            .OrderBy(x => x.Permission.Name)
            .ToListAsync();
    }

    public async Task<RolePermission?> GetByIdAsync(Guid id)
    {
        return await _context.RolePermissions
            .Include(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Delete(RolePermission rolePermission)
    {
        _context.RolePermissions.Remove(rolePermission);
    }
}
