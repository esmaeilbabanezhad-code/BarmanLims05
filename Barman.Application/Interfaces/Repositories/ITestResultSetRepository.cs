using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestResultSetRepository
{
    Task<List<TestResultSet>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default);

    Task<TestResultSet?> ResolveAsync(
            Guid testId,
            Guid? customerId,
            Guid? sampleCategoryId,
            Guid? matrixId,
            Guid? standardSampleId,
            CancellationToken cancellationToken = default);

    Task<TestResultSet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TestResultSet entity,
        CancellationToken cancellationToken = default);

    void Update(TestResultSet entity);

    void Delete(TestResultSet entity);
}