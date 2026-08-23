using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    // ============================================================
    // Employee
    // ============================================================

    public async Task<List<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.View");

        return await _unitOfWork.Employees.GetAllAsync();
    }

    public async Task<Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.View");

        return await _unitOfWork.Employees.GetByIdAsync(id);
    }

    public async Task<List<Employee>> GetByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.View");

        return await _unitOfWork.Employees
            .GetByDepartmentAsync(departmentId);
    }

    public async Task<Employee> CreateAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Create");

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee;
    }

    public async Task<Employee?> UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var existing = await _unitOfWork.Employees
            .GetByIdAsync(employee.Id);

        if (existing is null)
            return null;

        existing.PersonnelCode = employee.PersonnelCode;
        existing.FullName = employee.FullName;
        existing.NationalCode = employee.NationalCode;
        existing.Mobile = employee.Mobile;
        existing.Email = employee.Email;
        existing.CanApprove = employee.CanApprove;
        existing.CanReview = employee.CanReview;
        existing.IsSystemUser = employee.IsSystemUser;
        existing.IsActive = employee.IsActive;

        existing.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Employees.Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Deactivate");

        var employee =
            await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is null)
            return false;

        employee.IsDeleted = true;
        employee.IsActive = false;
        employee.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Employees.Update(employee);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<Employee> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var employee =
            await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is null)
            throw new InvalidOperationException(
                "Employee not found.");

        employee.IsActive = true;
        employee.IsDeleted = false;
        employee.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Employees.Update(employee);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee;
    }

    public async Task<Employee> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Deactivate");

        var employee =
            await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is null)
            throw new InvalidOperationException(
                "Employee not found.");

        employee.IsActive = false;
        employee.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Employees.Update(employee);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee;
    }

    // ============================================================
    // Employee Departments
    // ============================================================

    public async Task<List<EmployeeDepartment>> GetEmployeeDepartmentsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.View");

        return await _unitOfWork.EmployeeDepartments
            .GetByEmployeeIdAsync(employeeId);
    }

    public async Task<EmployeeDepartment> AddDepartmentAsync(
        EmployeeDepartment employeeDepartment,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var existingDepartments =
            await _unitOfWork.EmployeeDepartments
                .GetByEmployeeIdAsync(employeeDepartment.EmployeeId);

        if (existingDepartments.Any(x =>
            x.DepartmentId == employeeDepartment.DepartmentId))
        {
            throw new InvalidOperationException(
                "This department is already assigned to the employee.");
        }

        if (!existingDepartments.Any())
        {
            employeeDepartment.IsPrimary = true;
        }

        if (employeeDepartment.IsPrimary)
        {
            foreach (var item in existingDepartments)
            {
                item.IsPrimary = false;
                _unitOfWork.EmployeeDepartments.Update(item);
            }
        }

        await _unitOfWork.EmployeeDepartments
            .AddAsync(employeeDepartment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employeeDepartment;
    }

    public async Task<bool> RemoveDepartmentAsync(
        Guid employeeDepartmentId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var employeeDepartment =
            await _unitOfWork.EmployeeDepartments
                .GetByIdAsync(employeeDepartmentId);

        if (employeeDepartment is null)
            return false;

        var employeeId = employeeDepartment.EmployeeId;
        var wasPrimary = employeeDepartment.IsPrimary;

        _unitOfWork.EmployeeDepartments.Delete(employeeDepartment);

        if (wasPrimary)
        {
            var remaining =
                await _unitOfWork.EmployeeDepartments
                    .GetByEmployeeIdAsync(employeeId);

            var next = remaining
                .FirstOrDefault(x => x.Id != employeeDepartmentId);

            if (next is not null)
            {
                next.IsPrimary = true;
                _unitOfWork.EmployeeDepartments.Update(next);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SetPrimaryDepartmentAsync(
        Guid employeeId,
        Guid employeeDepartmentId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var departments =
            await _unitOfWork.EmployeeDepartments
                .GetByEmployeeIdAsync(employeeId);

        var selected = departments
            .FirstOrDefault(x => x.Id == employeeDepartmentId);

        if (selected is null)
            return false;

        foreach (var department in departments)
        {
            department.IsPrimary =
                department.Id == employeeDepartmentId;

            _unitOfWork.EmployeeDepartments.Update(department);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // ============================================================
    // Employee Roles
    // ============================================================

    public async Task<List<EmployeeRole>> GetEmployeeRolesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.View");

        return await _unitOfWork.EmployeeRoles
            .GetByEmployeeIdAsync(employeeId);
    }

    public async Task<EmployeeRole> AddRoleAsync(
        EmployeeRole employeeRole,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var existingRoles =
            await _unitOfWork.EmployeeRoles
                .GetByEmployeeIdAsync(employeeRole.EmployeeId);

        if (existingRoles.Any(x =>
            x.RoleId == employeeRole.RoleId))
        {
            throw new InvalidOperationException(
                "This role is already assigned to the employee.");
        }

        if (!existingRoles.Any())
        {
            employeeRole.IsPrimary = true;
        }

        if (employeeRole.IsPrimary)
        {
            foreach (var item in existingRoles)
            {
                item.IsPrimary = false;
                _unitOfWork.EmployeeRoles.Update(item);
            }
        }

        await _unitOfWork.EmployeeRoles.AddAsync(employeeRole);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employeeRole;
    }

    public async Task<bool> RemoveRoleAsync(
        Guid employeeRoleId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var employeeRole =
            await _unitOfWork.EmployeeRoles
                .GetByIdAsync(employeeRoleId);

        if (employeeRole is null)
            return false;

        var employeeId = employeeRole.EmployeeId;
        var wasPrimary = employeeRole.IsPrimary;

        _unitOfWork.EmployeeRoles.Delete(employeeRole);

        if (wasPrimary)
        {
            var remaining =
                await _unitOfWork.EmployeeRoles
                    .GetByEmployeeIdAsync(employeeId);

            var next = remaining
                .FirstOrDefault(x => x.Id != employeeRoleId);

            if (next is not null)
            {
                next.IsPrimary = true;
                _unitOfWork.EmployeeRoles.Update(next);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SetPrimaryRoleAsync(
        Guid employeeId,
        Guid employeeRoleId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePermissionAsync("Employee.Edit");

        var roles =
            await _unitOfWork.EmployeeRoles
                .GetByEmployeeIdAsync(employeeId);

        var selected = roles
            .FirstOrDefault(x => x.Id == employeeRoleId);

        if (selected is null)
            return false;

        foreach (var role in roles)
        {
            role.IsPrimary =
                role.Id == employeeRoleId;

            _unitOfWork.EmployeeRoles.Update(role);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // ============================================================
    // Permission
    // ============================================================

    private async Task EnsurePermissionAsync(
        string permissionCode)
    {
        var allowed =
            await _permissionService.HasPermissionAsync(
                permissionCode);

        if (!allowed)
        {
            throw new UnauthorizedAccessException(
                $"Permission denied: {permissionCode}");
        }
    }
}