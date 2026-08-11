using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IMatrixRepository
{
    Task<List<Matrix>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Matrix?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Matrix>> GetBySampleCategoryIdAsync(
        Guid sampleCategoryId,
        CancellationToken cancellationToken = default);

    Task<List<Matrix>> GetDeletedAsync(
    CancellationToken cancellationToken = default);

    Task<Matrix?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Matrix matrix,
        CancellationToken cancellationToken = default);

    void Update(Matrix matrix);

    void Delete(Matrix matrix);
}