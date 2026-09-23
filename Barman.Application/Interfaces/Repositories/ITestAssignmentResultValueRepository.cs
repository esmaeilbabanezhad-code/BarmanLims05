using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestAssignmentResultValueRepository
{
    Task<List<TestAssignmentResultValue>> GetByAssignmentIdAsync(
        Guid testAssignmentId);

    Task<TestAssignmentResultValue?> GetByAssignmentAndItemAsync(
        Guid testAssignmentId,
        Guid testResultSetItemId);

    Task<List<TestAssignmentResultValue>> GetByAssignmentIdsAsync(
    IReadOnlyCollection<Guid> assignmentIds);

    Task<TestAssignmentResultValue?> GetByIdAsync(
    Guid id);
    Task AddAsync(TestAssignmentResultValue resultValue);

    void Update(TestAssignmentResultValue resultValue);

    void Delete(TestAssignmentResultValue resultValue);
}
