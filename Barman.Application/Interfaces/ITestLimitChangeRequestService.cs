using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestLimitChangeRequestService
{
    Task<TestLimitChangeRequest> CreateRequestAsync(
        Guid testAssignmentId,
        Guid testId,
        Guid requestedByEmployeeId,
        Guid? referenceLimitId,
        decimal? requestedLOD,
        decimal? requestedLOQ,
        decimal? requestedMinValue,
        decimal? requestedMaxValue,
        decimal? requestedWarningLow,
        decimal? requestedWarningHigh,
        string? reason);

    Task<TestLimitChangeRequest>
     CreateResultSetItemRequestAsync(
         Guid testAssignmentId,
         Guid testResultSetItemId,
         Guid requestedByEmployeeId,
         decimal? requestedLOD,
         decimal? requestedLOQ,
         decimal? requestedMinValue,
         decimal? requestedMaxValue,
         string? reason);

    Task<List<TestLimitChangeRequest>>
        GetPendingForSectionHeadAsync(Guid departmentId);

    Task ApproveAsync(
        Guid requestId,
        Guid approvedByEmployeeId,
        string? approvalComment);

    Task RejectAsync(
        Guid requestId,
        Guid approvedByEmployeeId,
        string? approvalComment);
}