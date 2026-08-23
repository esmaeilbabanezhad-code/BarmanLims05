using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public DepartmentService(
        IUnitOfWork unitOfWork,
        IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        await EnsurePermissionAsync("Department.View");

        return await _unitOfWork.Departments.GetAllAsync();
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        await EnsurePermissionAsync("Department.View");

        return await _unitOfWork.Departments.GetByIdAsync(id);
    }

    public async Task<Department> CreateAsync(Department department)
    {
        await EnsurePermissionAsync("Department.Create");

        var list = await _unitOfWork.Departments.GetAllAsync();

        if (list.Any(x => x.Code == department.Code))
            throw new Exception("Department Code already exists.");

        if (list.Any(x => x.Name == department.Name))
            throw new Exception("Department Name already exists.");

        department.IsActive = true;

        await _unitOfWork.Departments.AddAsync(department);

        await _unitOfWork.SaveChangesAsync();

        return department;
    }

    public async Task<Department?> UpdateAsync(Department department)
    {
        await EnsurePermissionAsync("Department.Edit");

        var current =
            await _unitOfWork.Departments.GetByIdAsync(department.Id);

        if (current == null)
            return null;

        current.Code = department.Code;
        current.Name = department.Name;
        current.Description = department.Description;
        current.IsActive = department.IsActive;

        _unitOfWork.Departments.Update(current);

        await _unitOfWork.SaveChangesAsync();

        return current;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsurePermissionAsync("Department.Delete");

        var current =
            await _unitOfWork.Departments.GetByIdAsync(id);

        if (current == null)
            return false;

        _unitOfWork.Departments.Delete(current);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<Department> ActivateAsync(Guid id)
    {
        await EnsurePermissionAsync("Department.Edit");

        var department =
            await _unitOfWork.Departments.GetByIdAsync(id);

        if (department == null)
            throw new KeyNotFoundException(
                "Department not found.");

        department.IsActive = true;

        _unitOfWork.Departments.Update(department);

        await _unitOfWork.SaveChangesAsync();

        return department;
    }

    public async Task<Department> DeactivateAsync(Guid id)
    {
        await EnsurePermissionAsync("Department.Edit");

        var department =
            await _unitOfWork.Departments.GetByIdAsync(id);

        if (department == null)
            throw new KeyNotFoundException(
                "Department not found.");

        department.IsActive = false;

        _unitOfWork.Departments.Update(department);

        await _unitOfWork.SaveChangesAsync();

        return department;
    }

    private async Task EnsurePermissionAsync(string permissionCode)
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