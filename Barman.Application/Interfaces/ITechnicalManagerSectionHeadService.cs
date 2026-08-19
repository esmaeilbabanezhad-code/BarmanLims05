using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITechnicalManagerSectionHeadService
{
    Task<List<TechnicalManagerSectionHead>>
        GetByTechnicalManagerIdAsync(
            Guid technicalManagerId,
            CancellationToken cancellationToken = default);

    Task<List<TechnicalManagerSectionHead>>
        GetBySectionHeadIdAsync(
            Guid sectionHeadId,
            CancellationToken cancellationToken = default);

    Task<TechnicalManagerSectionHead?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerSectionHead> AddAsync(
        TechnicalManagerSectionHead relation,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        TechnicalManagerSectionHead relation,
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
