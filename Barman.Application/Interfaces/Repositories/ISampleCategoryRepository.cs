using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ISampleCategoryRepository
{
    Task AddAsync(
        SampleCategory category);

    Task<List<SampleCategory>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<SampleCategory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SampleCategory?> GetByCodeAsync(
    string code,
    CancellationToken cancellationToken = default);

    Task<List<SampleCategory>> GetDeletedAsync(
    CancellationToken cancellationToken = default);

    Task<SampleCategory?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    void Update(SampleCategory category);

    void Delete(SampleCategory category);
}
