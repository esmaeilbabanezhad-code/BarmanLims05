using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class DepartmentResponsibilityRepository
    : IDepartmentResponsibilityRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentResponsibilityRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentResponsibility>> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        return await _context.Set<DepartmentResponsibility>()
            .Include(x => x.Employee)
            .Include(x => x.Department)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<DepartmentResponsibility>> GetByDepartmentIdAsync(
        Guid departmentId)
    {
        return await _context.Set<DepartmentResponsibility>()
            .Include(x => x.Employee)
            .Include(x => x.Department)
            .Where(x => x.DepartmentId == departmentId)
            .ToListAsync();
    }

    public async Task<DepartmentResponsibility?> GetByIdAsync(
        Guid id)
    {
        return await _context.Set<DepartmentResponsibility>()
            .Include(x => x.Employee)
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(
        DepartmentResponsibility responsibility)
    {
        await _context.Set<DepartmentResponsibility>()
            .AddAsync(responsibility);
    }

    public void Update(
        DepartmentResponsibility responsibility)
    {
        _context.Set<DepartmentResponsibility>()
            .Update(responsibility);
    }

    public void Delete(
        DepartmentResponsibility responsibility)
    {
        _context.Set<DepartmentResponsibility>()
            .Remove(responsibility);
    }
}
