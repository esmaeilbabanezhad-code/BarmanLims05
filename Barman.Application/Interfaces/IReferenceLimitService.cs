using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IReferenceLimitService
{
    Task<List<ReferenceLimit>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default);

    Task<ReferenceLimit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ReferenceLimit> CreateAsync(
        ReferenceLimit referenceLimit,
        CancellationToken cancellationToken = default);

    Task<ReferenceLimit?> UpdateAsync(
        ReferenceLimit referenceLimit,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default);
}