using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IDefaultTestSetRepository
{
    Task<List<DefaultTestSet>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<List<DefaultTestSet>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<DefaultTestSet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DefaultTestSet?> ResolveAsync(
    Guid? customerId,
    Guid? sampleCategoryId,
    Guid? matrixId,
    Guid? standardSampleId,
    CancellationToken cancellationToken = default);

    Task AddAsync(
        DefaultTestSet entity,
        CancellationToken cancellationToken = default);

    void Update(DefaultTestSet entity);

    void Delete(DefaultTestSet entity);
}