using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestResultSetItemRepository
{
    Task<List<TestResultSetItem>> GetByResultSetIdAsync(
        Guid testResultSetId,
        CancellationToken cancellationToken = default);

    Task<TestResultSetItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TestResultSetItem entity,
        CancellationToken cancellationToken = default);

    void Update(TestResultSetItem entity);

    void Delete(TestResultSetItem entity);
}