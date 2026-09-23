using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestAssignmentResultValueService
    : ITestAssignmentResultValueService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestAssignmentResultValueService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestAssignmentResultValue>>
        GetByAssignmentIdAsync(
            Guid testAssignmentId,
            CancellationToken cancellationToken = default)
    {
        if (testAssignmentId == Guid.Empty)
            return new List<TestAssignmentResultValue>();

        return await _unitOfWork.TestAssignmentResultValues
            .GetByAssignmentIdAsync(testAssignmentId);
    }

    public async Task<TestAssignmentResultValue?>
        GetByAssignmentAndItemAsync(
            Guid testAssignmentId,
            Guid testResultSetItemId,
            CancellationToken cancellationToken = default)
    {
        if (testAssignmentId == Guid.Empty ||
            testResultSetItemId == Guid.Empty)
        {
            return null;
        }

        return await _unitOfWork.TestAssignmentResultValues
            .GetByAssignmentAndItemAsync(
                testAssignmentId,
                testResultSetItemId);
    }

    public async Task<TestAssignmentResultValue>
        SaveAsync(
            Guid testAssignmentId,
            Guid testResultSetItemId,
            string? value,
            string? comment,
            CancellationToken cancellationToken = default)
    {
        if (testAssignmentId == Guid.Empty)
            throw new ArgumentException(
                "TestAssignmentId is required.");

        if (testResultSetItemId == Guid.Empty)
            throw new ArgumentException(
                "TestResultSetItemId is required.");

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(testAssignmentId);

        if (assignment == null || assignment.IsDeleted)
            throw new KeyNotFoundException(
                "Test assignment not found.");

        var resultSetItem =
            await _unitOfWork.TestResultSetItems
                .GetByIdAsync(testResultSetItemId);

        if (resultSetItem == null || resultSetItem.IsDeleted)
            throw new KeyNotFoundException(
                "Test result set item not found.");

        if (resultSetItem.TestResultSet == null)
            throw new InvalidOperationException(
                "Result set information is not available.");

        if (resultSetItem.TestResultSet.TestId !=
            assignment.TestId)
        {
            throw new InvalidOperationException(
                "Result set item does not belong to the assigned test.");
        }

        var existing =
            await _unitOfWork.TestAssignmentResultValues
                .GetByAssignmentAndItemAsync(
                    testAssignmentId,
                    testResultSetItemId);

        if (existing == null)
        {
            existing = new TestAssignmentResultValue
            {
                Id = Guid.NewGuid(),
                TestAssignmentId = testAssignmentId,
                TestResultSetItemId = testResultSetItemId,
                Value = string.IsNullOrWhiteSpace(value)
                    ? null
                    : value.Trim(),
                Comment = string.IsNullOrWhiteSpace(comment)
                    ? null
                    : comment.Trim(),
                IsDeleted = false,
                IsActive = true
            };

            await _unitOfWork.TestAssignmentResultValues
                .AddAsync(existing);
        }
        else
        {
            existing.Value =
                string.IsNullOrWhiteSpace(value)
                    ? null
                    : value.Trim();

            existing.Comment =
                string.IsNullOrWhiteSpace(comment)
                    ? null
                    : comment.Trim();

            existing.IsDeleted = false;
            existing.IsActive = true;

            _unitOfWork.TestAssignmentResultValues
                .Update(existing);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return existing;
    }

    public async Task DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return;

        var entity =
            await _unitOfWork.TestAssignmentResultValues
                .GetByIdAsync(id);

        if (entity == null)
            return;

        _unitOfWork.TestAssignmentResultValues
            .Delete(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
