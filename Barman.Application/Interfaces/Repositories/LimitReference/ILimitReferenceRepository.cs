using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ILimitReferenceRepository
{
    Task AddAsync(
        LimitReference limitReference);

    Task<List<LimitReference>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<LimitReference?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<LimitReference?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<List<LimitReference>> GetDeletedAsync(
        CancellationToken cancellationToken = default);

    Task<LimitReference?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    void Update(LimitReference limitReference);

    void Delete(LimitReference limitReference);
}
