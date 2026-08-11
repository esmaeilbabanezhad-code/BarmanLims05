using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id);

    Task<List<Department>> GetAllAsync();

    Task<List<Department>> GetActiveAsync();

    Task AddAsync(Department department);

    void Update(Department department);

    void Delete(Department department);
}