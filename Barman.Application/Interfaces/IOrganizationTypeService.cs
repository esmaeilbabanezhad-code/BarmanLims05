using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IOrganizationTypeService
{
    Task<List<OrganizationType>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<OrganizationType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<OrganizationType>> GetDeletedAsync(
        CancellationToken cancellationToken = default);

    Task<OrganizationType?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OrganizationType> CreateAsync(
        OrganizationType organizationType,
        CancellationToken cancellationToken = default);

    Task<OrganizationType> UpdateAsync(
        Guid id,
        OrganizationType organizationType,
        CancellationToken cancellationToken = default);

    Task<OrganizationType> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OrganizationType> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OrganizationType> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OrganizationType> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}