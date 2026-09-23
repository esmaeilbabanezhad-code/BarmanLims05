using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TechnicalManagerScopeDepartmentRepository
    : ITechnicalManagerScopeDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public TechnicalManagerScopeDepartmentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TechnicalManagerScopeDepartment>>
        GetByScopeIdAsync(Guid scopeId)
    {
        return await _context.TechnicalManagerScopeDepartments
            .Include(x => x.TechnicalManagerScope)
            .Include(x => x.Department)
            .Where(x =>
                x.TechnicalManagerScopeId == scopeId &&
                !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Department.Name)
            .ToListAsync();
    }

    public async Task<List<TechnicalManagerScopeDepartment>>
        GetByDepartmentIdAsync(Guid departmentId)
    {
        return await _context.TechnicalManagerScopeDepartments
            .Include(x => x.TechnicalManagerScope)
            .Include(x => x.Department)
            .Where(x =>
                x.DepartmentId == departmentId &&
                !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.TechnicalManagerScope.Name)
            .ToListAsync();
    }

    public async Task<TechnicalManagerScopeDepartment?>
        GetByIdAsync(Guid id)
    {
        return await _context.TechnicalManagerScopeDepartments
            .Include(x => x.TechnicalManagerScope)
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<bool> ExistsAsync(
        Guid scopeId,
        Guid departmentId)
    {
        return await _context.TechnicalManagerScopeDepartments
            .AnyAsync(x =>
                x.TechnicalManagerScopeId == scopeId &&
                x.DepartmentId == departmentId &&
                !x.IsDeleted);
    }

    public async Task AddAsync(
        TechnicalManagerScopeDepartment relation)
    {
        await _context.TechnicalManagerScopeDepartments
            .AddAsync(relation);
    }

    public void Update(
        TechnicalManagerScopeDepartment relation)
    {
        _context.TechnicalManagerScopeDepartments.Update(relation);
    }

    public void Delete(
        TechnicalManagerScopeDepartment relation)
    {
        _context.TechnicalManagerScopeDepartments.Remove(relation);
    }
}
