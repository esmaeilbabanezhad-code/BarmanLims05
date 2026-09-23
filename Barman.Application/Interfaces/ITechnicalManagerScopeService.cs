using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITechnicalManagerScopeService
{
    Task<List<TechnicalManagerScope>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScope?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScope?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScope> AddAsync(
        TechnicalManagerScope scope,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        TechnicalManagerScope scope,
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
