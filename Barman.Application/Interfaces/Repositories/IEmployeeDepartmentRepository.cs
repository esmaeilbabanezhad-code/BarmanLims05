using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IEmployeeDepartmentRepository
{
    Task AddAsync(EmployeeDepartment employeeDepartment);

    Task<List<EmployeeDepartment>> GetByEmployeeIdAsync(Guid employeeId);

    Task<EmployeeDepartment?> GetByIdAsync(Guid id);

    Task<bool> ExistsAsync(Guid employeeId, Guid departmentId);

    void Update(EmployeeDepartment employeeDepartment);

    void Delete(EmployeeDepartment employeeDepartment);
}
