using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class EmployeeTechnicalManagerScopeRepository
    : IEmployeeTechnicalManagerScopeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeTechnicalManagerScopeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeTechnicalManagerScope>> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        return await _context.Set<EmployeeTechnicalManagerScope>()
            .Include(x => x.Employee)
            .Include(x => x.TechnicalManagerScope)
            .Where(x => x.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<List<EmployeeTechnicalManagerScope>> GetByScopeIdAsync(
        Guid scopeId)
    {
        return await _context.Set<EmployeeTechnicalManagerScope>()
            .Include(x => x.Employee)
            .Include(x => x.TechnicalManagerScope)
            .Where(x => x.TechnicalManagerScopeId == scopeId)
            .ToListAsync();
    }

    public async Task<EmployeeTechnicalManagerScope?> GetByIdAsync(
        Guid id)
    {
        return await _context.Set<EmployeeTechnicalManagerScope>()
            .Include(x => x.Employee)
            .Include(x => x.TechnicalManagerScope)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(
        EmployeeTechnicalManagerScope scope)
    {
        await _context.Set<EmployeeTechnicalManagerScope>()
            .AddAsync(scope);
    }

    public void Update(
        EmployeeTechnicalManagerScope scope)
    {
        _context.Set<EmployeeTechnicalManagerScope>()
            .Update(scope);
    }

    public void Delete(
        EmployeeTechnicalManagerScope scope)
    {
        _context.Set<EmployeeTechnicalManagerScope>()
            .Remove(scope);
    }
}