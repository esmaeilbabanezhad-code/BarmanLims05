using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Employee>> GetByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<Employee> CreateAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task<Employee?> UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Employee> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Employee> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Departments

    Task<List<EmployeeDepartment>> GetEmployeeDepartmentsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<EmployeeDepartment> AddDepartmentAsync(
        EmployeeDepartment employeeDepartment,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveDepartmentAsync(
        Guid employeeDepartmentId,
        CancellationToken cancellationToken = default);

    Task<bool> SetPrimaryDepartmentAsync(
        Guid employeeId,
        Guid employeeDepartmentId,
        CancellationToken cancellationToken = default);

    // Roles

    Task<List<EmployeeRole>> GetEmployeeRolesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<EmployeeRole> AddRoleAsync(
        EmployeeRole employeeRole,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveRoleAsync(
        Guid employeeRoleId,
        CancellationToken cancellationToken = default);

    Task<bool> SetPrimaryRoleAsync(
        Guid employeeId,
        Guid employeeRoleId,
        CancellationToken cancellationToken = default);
}