using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITechnicalManagerScopeDepartmentService
{
    Task<List<TechnicalManagerScopeDepartment>> GetByScopeIdAsync(
        Guid scopeId,
        CancellationToken cancellationToken = default);

    Task<List<TechnicalManagerScopeDepartment>> GetByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScopeDepartment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScopeDepartment> AddAsync(
        TechnicalManagerScopeDepartment relation,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        TechnicalManagerScopeDepartment relation,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
