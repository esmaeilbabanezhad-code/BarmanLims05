using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITechnicalManagerScopeDepartmentRepository
{
    Task<List<TechnicalManagerScopeDepartment>> GetByScopeIdAsync(
        Guid scopeId);

    Task<List<TechnicalManagerScopeDepartment>> GetByDepartmentIdAsync(
        Guid departmentId);

    Task<TechnicalManagerScopeDepartment?> GetByIdAsync(
        Guid id);

    Task<bool> ExistsAsync(
        Guid scopeId,
        Guid departmentId);

    Task AddAsync(
        TechnicalManagerScopeDepartment relation);

    void Update(
        TechnicalManagerScopeDepartment relation);

    void Delete(
        TechnicalManagerScopeDepartment relation);
}
