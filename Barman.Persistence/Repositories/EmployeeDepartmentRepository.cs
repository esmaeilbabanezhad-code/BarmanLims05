using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class EmployeeDepartmentRepository : IEmployeeDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeDepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EmployeeDepartment employeeDepartment)
    {
        await _context.EmployeeDepartments.AddAsync(employeeDepartment);
    }

    public async Task<List<EmployeeDepartment>> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        return await _context.EmployeeDepartments
            .Include(x => x.Department)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Department.Name)
            .ToListAsync();
    }

    public async Task<EmployeeDepartment?> GetByIdAsync(Guid id)
    {
        return await _context.EmployeeDepartments
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(
        Guid employeeId,
        Guid departmentId)
    {
        return await _context.EmployeeDepartments
            .AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.DepartmentId == departmentId);
    }

    public void Update(EmployeeDepartment employeeDepartment)
    {
        _context.EmployeeDepartments.Update(employeeDepartment);
    }

    public void Delete(EmployeeDepartment employeeDepartment)
    {
        _context.EmployeeDepartments.Remove(employeeDepartment);
    }
}
