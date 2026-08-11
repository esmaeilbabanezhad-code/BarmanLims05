using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<Department>> GetAllAsync();

    Task<Department?> GetByIdAsync(Guid id);

    Task<Department> CreateAsync(Department department);

    Task<Department?> UpdateAsync(Department department);

    Task<bool> DeleteAsync(Guid id);

    Task<Department> ActivateAsync(Guid id);

    Task<Department> DeactivateAsync(Guid id);
}