using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await _unitOfWork.Departments.GetAllAsync();
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.Departments.GetByIdAsync(id);
    }

    public async Task<Department> CreateAsync(Department department)
    {
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
}