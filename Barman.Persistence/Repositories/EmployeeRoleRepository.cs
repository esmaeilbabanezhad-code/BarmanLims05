using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class EmployeeRoleRepository : IEmployeeRoleRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EmployeeRole employeeRole)
    {
        await _context.EmployeeRoles.AddAsync(employeeRole);
    }

    public async Task<List<EmployeeRole>> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        return await _context.EmployeeRoles
            .Include(x => x.Role)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Role.Name)
            .ToListAsync();
    }

    public async Task<EmployeeRole?> GetByIdAsync(Guid id)
    {
        return await _context.EmployeeRoles
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Update(EmployeeRole employeeRole)
    {
        _context.EmployeeRoles.Update(employeeRole);
    }

    public void Delete(EmployeeRole employeeRole)
    {
        _context.EmployeeRoles.Remove(employeeRole);
    }
}