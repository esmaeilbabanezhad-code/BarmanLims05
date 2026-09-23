using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITechnicalManagerScopeRepository
{
    Task<List<TechnicalManagerScope>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScope?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerScope?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TechnicalManagerScope scope,
        CancellationToken cancellationToken = default);

    void Update(
        TechnicalManagerScope scope);

    void Delete(
        TechnicalManagerScope scope);
}