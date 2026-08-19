using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IDepartmentResponsibilityService
{
    Task<List<DepartmentResponsibility>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<List<DepartmentResponsibility>> GetByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<DepartmentResponsibility?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DepartmentResponsibility> AddAsync(
        DepartmentResponsibility responsibility,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        DepartmentResponsibility responsibility,
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
