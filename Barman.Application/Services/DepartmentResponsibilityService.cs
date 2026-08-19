using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class DepartmentResponsibilityService
    : IDepartmentResponsibilityService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentResponsibilityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DepartmentResponsibility>>
        GetByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.DepartmentResponsibilities
            .GetByEmployeeIdAsync(employeeId);
    }

    public async Task<List<DepartmentResponsibility>>
        GetByDepartmentIdAsync(
            Guid departmentId,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.DepartmentResponsibilities
            .GetByDepartmentIdAsync(departmentId);
    }

    public async Task<DepartmentResponsibility?>
        GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.DepartmentResponsibilities
            .GetByIdAsync(id);
    }

    public async Task<DepartmentResponsibility> AddAsync(
        DepartmentResponsibility responsibility,
        CancellationToken cancellationToken = default)
    {
        var isEmployeeInDepartment =
            await _unitOfWork.EmployeeDepartments
                .ExistsAsync(
                    responsibility.EmployeeId,
                    responsibility.DepartmentId);

        if (!isEmployeeInDepartment)
        {
            throw new InvalidOperationException(
                "The employee is not assigned to this department.");
        }

        var existing =
            await _unitOfWork.DepartmentResponsibilities
                .GetByDepartmentIdAsync(
                    responsibility.DepartmentId);

        if (existing.Any(x =>
            x.EmployeeId == responsibility.EmployeeId &&
            x.ResponsibilityType == responsibility.ResponsibilityType))
        {
            throw new InvalidOperationException(
                "This responsibility is already assigned to this employee in the department.");
        }

        if (!existing.Any(x =>
            x.ResponsibilityType == responsibility.ResponsibilityType))
        {
            responsibility.IsPrimary = true;
        }

        if (responsibility.IsPrimary)
        {
            foreach (var item in existing.Where(x =>
                x.ResponsibilityType == responsibility.ResponsibilityType))
            {
                item.IsPrimary = false;
                _unitOfWork.DepartmentResponsibilities.Update(item);
            }
        }

        await _unitOfWork.DepartmentResponsibilities
            .AddAsync(responsibility);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return responsibility;
    }

    public async Task<bool> UpdateAsync(
        DepartmentResponsibility responsibility,
        CancellationToken cancellationToken = default)
    {
        var existing =
            await _unitOfWork.DepartmentResponsibilities
                .GetByIdAsync(responsibility.Id);

        if (existing is null)
            return false;

        var isEmployeeInDepartment =
            await _unitOfWork.EmployeeDepartments
                .ExistsAsync(
                    responsibility.EmployeeId,
                    responsibility.DepartmentId);

        if (!isEmployeeInDepartment)
        {
            throw new InvalidOperationException(
                "The employee is not assigned to this department.");
        }

        var responsibilities =
            await _unitOfWork.DepartmentResponsibilities
                .GetByDepartmentIdAsync(
                    responsibility.DepartmentId);

        if (responsibilities.Any(x =>
            x.Id != responsibility.Id &&
            x.EmployeeId == responsibility.EmployeeId &&
            x.ResponsibilityType == responsibility.ResponsibilityType))
        {
            throw new InvalidOperationException(
                "This responsibility is already assigned to this employee in the department.");
        }

        if (responsibility.IsPrimary)
        {
            foreach (var item in responsibilities.Where(x =>
                x.Id != responsibility.Id &&
                x.ResponsibilityType == responsibility.ResponsibilityType))
            {
                item.IsPrimary = false;
                _unitOfWork.DepartmentResponsibilities.Update(item);
            }
        }

        existing.EmployeeId = responsibility.EmployeeId;
        existing.DepartmentId = responsibility.DepartmentId;
        existing.ResponsibilityType = responsibility.ResponsibilityType;
        existing.IsPrimary = responsibility.IsPrimary;
        existing.IsActive = responsibility.IsActive;

        existing.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.DepartmentResponsibilities.Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var responsibility =
            await _unitOfWork.DepartmentResponsibilities
                .GetByIdAsync(id);

        if (responsibility is null)
            return false;

        responsibility.IsDeleted = true;
        responsibility.IsActive = false;
        responsibility.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.DepartmentResponsibilities.Update(responsibility);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var responsibility =
            await _unitOfWork.DepartmentResponsibilities
                .GetByIdAsync(id);

        if (responsibility is null)
            return false;

        responsibility.IsActive = true;
        responsibility.IsDeleted = false;
        responsibility.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.DepartmentResponsibilities.Update(responsibility);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var responsibility =
            await _unitOfWork.DepartmentResponsibilities
                .GetByIdAsync(id);

        if (responsibility is null)
            return false;

        responsibility.IsActive = false;
        responsibility.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.DepartmentResponsibilities.Update(responsibility);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
