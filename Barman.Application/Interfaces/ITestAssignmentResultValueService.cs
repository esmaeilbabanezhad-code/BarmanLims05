using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestAssignmentResultValueService
{
    Task<List<TestAssignmentResultValue>> GetByAssignmentIdAsync(
        Guid testAssignmentId,
        CancellationToken cancellationToken = default);

    Task<TestAssignmentResultValue?> GetByAssignmentAndItemAsync(
        Guid testAssignmentId,
        Guid testResultSetItemId,
        CancellationToken cancellationToken = default);

    Task<TestAssignmentResultValue> SaveAsync(
        Guid testAssignmentId,
        Guid testResultSetItemId,
        string? value,
        string? comment,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
