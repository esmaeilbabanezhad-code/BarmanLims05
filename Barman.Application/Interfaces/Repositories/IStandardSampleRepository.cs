using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IStandardSampleRepository
{
    Task<List<StandardSample>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StandardSample?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<StandardSample>> GetByMatrixIdAsync(
        Guid matrixId,
        CancellationToken cancellationToken = default);

    Task<List<StandardSample>> GetDeletedAsync(
        CancellationToken cancellationToken = default);

    Task<StandardSample?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        StandardSample standardSample,
        CancellationToken cancellationToken = default);

    void Update(StandardSample standardSample);

    void Delete(StandardSample standardSample);
}