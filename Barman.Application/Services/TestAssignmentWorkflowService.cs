using Barman.Application.Interfaces;
using Barman.Domain.Entities;
using Barman.Domain.Enums;

namespace Barman.Application.Services;

public class TestAssignmentWorkflowService
    : ITestAssignmentWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public TestAssignmentWorkflowService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task AssignToSectionAsync(
        Guid assignmentId,
        Guid departmentId)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.TechnicalManagerAssignment);

        if (departmentId == Guid.Empty)
            throw new InvalidOperationException(
                "بخش مقصد مشخص نشده است.");

        assignment.DepartmentId = departmentId;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.SectionAssignment;

        _unitOfWork.TestAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAssignmentDepartmentAsync(
    Guid assignmentId,
    Guid departmentId)
    {
        if (assignmentId == Guid.Empty)
            throw new InvalidOperationException(
                "شناسه Assignment معتبر نیست.");

        if (departmentId == Guid.Empty)
            throw new InvalidOperationException(
                "بخش مشخص نشده است.");

        var assignment =
            await GetAssignmentAsync(assignmentId);

        assignment.DepartmentId = departmentId;

        _unitOfWork.TestAssignments.Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignPanelToSectionAsync(
        Guid sampleId,
        Guid testPanelId,
        Guid departmentId)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty ||
            departmentId == Guid.Empty)
            throw new InvalidOperationException(
                "اطلاعات Panel یا بخش مقصد کامل نیست.");

        var assignments =
            await _unitOfWork.TestAssignments
                .GetBySampleIdAndTestPanelIdAsync(
                    sampleId,
                    testPanelId);

        if (assignments is null || assignments.Count == 0)
            throw new InvalidOperationException(
                "آزمون‌های Panel پیدا نشدند.");

        foreach (var assignment in assignments)
        {
            EnsureStage(
                assignment,
                TestAssignmentWorkflowStage.TechnicalManagerAssignment);

            assignment.DepartmentId = departmentId;
            assignment.WorkflowStage =
                TestAssignmentWorkflowStage.SectionAssignment;

            _unitOfWork.TestAssignments.Update(assignment);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignToAnalystAsync(
        Guid assignmentId,
        Guid analystId)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.SectionAssignment);

        if (analystId == Guid.Empty)
            throw new InvalidOperationException(
                "کارشناس مشخص نشده است.");

        assignment.AnalystId = analystId;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.AnalystWork;

        _unitOfWork.TestAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SubmitResultAsync(
        Guid assignmentId,
        string result)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.AnalystWork);

        if (string.IsNullOrWhiteSpace(result))
            throw new InvalidOperationException(
                "نتیجه آزمون نمی‌تواند خالی باشد.");

        assignment.Result = result;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.SectionResultApproval;

        _unitOfWork.TestAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task SubmitResultSetAsync(Guid assignmentId)
    {
        var assignment =
            await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.AnalystWork);

        if (!assignment.TestResultSetId.HasValue)
        {
            throw new InvalidOperationException(
                "برای این آزمون Result Set تعریف نشده است.");
        }

        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.SectionResultApproval;

        _unitOfWork.TestAssignments.Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SubmitSelectedAssignmentsToSectionAsync(
    List<Guid> assignmentIds)
    {
        if (assignmentIds is null ||
            assignmentIds.Count == 0)
            return;

        foreach (var assignmentId in assignmentIds.Distinct())
        {
            if (assignmentId == Guid.Empty)
                continue;

            var assignment =
                await GetAssignmentAsync(assignmentId);

            EnsureStage(
                assignment,
                TestAssignmentWorkflowStage.AnalystWork);

            bool hasResult;

            // =========================================================
            // ResultSet-based assignment
            // =========================================================
            if (assignment.TestResultSetId.HasValue &&
                assignment.TestResultSetId.Value != Guid.Empty)
            {
                var items =
                    await _unitOfWork.TestResultSetItems
                        .GetByResultSetIdAsync(
                            assignment.TestResultSetId.Value);

                var activeItems =
                    items
                        .Where(x => !x.IsDeleted)
                        .ToList();

                if (activeItems.Count == 0)
                {
                    hasResult = false;
                }
                else
                {
                    var requiredItems =
                        activeItems
                            .Where(x =>
                                x.TestResultDefinition.IsRequired)
                            .ToList();

                    var values =
                        await _unitOfWork.TestAssignmentResultValues
                            .GetByAssignmentIdAsync(
                                assignment.Id);

                    var enteredRequiredCount =
                        requiredItems.Count(item =>
                            values.Any(value =>
                                value.TestResultSetItemId == item.Id &&
                                !string.IsNullOrWhiteSpace(value.Value)));

                    hasResult =
                        enteredRequiredCount > 0;
                }
            }
            // =========================================================
            // Legacy single-result assignment
            // =========================================================
            else
            {
                hasResult =
                    !string.IsNullOrWhiteSpace(
                        assignment.Result);
            }

            // فقط آزمون‌هایی که هنوز هیچ نتیجه‌ای ندارند
            if (hasResult)
                continue;

            assignment.WorkflowStage =
                     TestAssignmentWorkflowStage.SectionAssignment;

            assignment.IsRejectedBySection = false;

            _unitOfWork.TestAssignments.Update(assignment);
        }

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ReturnSelectedAssignmentsToTechnicalManagerAsync(
    List<Guid> assignmentIds)
    {
        if (assignmentIds is null ||
            assignmentIds.Count == 0)
            return;

        foreach (var assignmentId in assignmentIds.Distinct())
        {
            if (assignmentId == Guid.Empty)
                continue;

            var assignment =
                await GetAssignmentAsync(assignmentId);

            EnsureStage(
                assignment,
                TestAssignmentWorkflowStage.SectionAssignment);

            assignment.WorkflowStage =
                TestAssignmentWorkflowStage.TechnicalManagerAssignment;

            _unitOfWork.TestAssignments.Update(assignment);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RequestReceptionCorrectionAsync(
    Guid receptionId,
    Guid sampleId,
    string reason)
    {
        if (receptionId == Guid.Empty)
            throw new InvalidOperationException(
                "پذیرش مشخص نشده است.");

        if (sampleId == Guid.Empty)
            throw new InvalidOperationException(
                "نمونه مشخص نشده است.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException(
                "دلیل درخواست اصلاح باید مشخص شود.");

        var currentEmployeeId =
            _currentUserService.EmployeeId;

        if (!currentEmployeeId.HasValue ||
            currentEmployeeId.Value == Guid.Empty)
        {
            throw new InvalidOperationException(
                "کاربر جاری مشخص نیست.");
        }

        var sample = await _unitOfWork
            .Samples
            .GetByIdAsync(sampleId);

        if (sample is null)
            throw new InvalidOperationException(
                "نمونه موردنظر پیدا نشد.");

        if (sample.ReceptionId != receptionId)
            throw new InvalidOperationException(
                "نمونه انتخاب‌شده متعلق به این پذیرش نیست.");

        var pendingRequest =
            await _unitOfWork
                .ReceptionCorrectionRequests
                .GetPendingByReceptionIdAsync(
                    receptionId);

        if (pendingRequest is not null)
            throw new InvalidOperationException(
                "برای این پذیرش قبلاً یک درخواست اصلاح در انتظار بررسی وجود دارد.");

        var request = new ReceptionCorrectionRequest
        {
            ReceptionId = receptionId,
            SampleId = sampleId,
            RequestedByEmployeeId =
                currentEmployeeId.Value,
            RequestedAt =
                DateTimeOffset.UtcNow,
            Reason =
                reason.Trim(),
            Status =
                ReceptionCorrectionRequestStatus.Pending
        };

        await _unitOfWork
    .ReceptionCorrectionRequests
    .AddAsync(request);

        var assignments =
            await _unitOfWork
                .TestAssignments
                .GetBySampleIdAsync(sampleId);

        if (assignments.Count == 0)
        {
            throw new InvalidOperationException(
                "برای نمونه انتخاب‌شده هیچ آزمونی پیدا نشد.");
        }

        foreach (var assignment in assignments)
        {
            assignment.WorkflowStage =
                TestAssignmentWorkflowStage.ReceptionCorrection;

            _unitOfWork.TestAssignments.Update(assignment);
        }

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApproveBySectionAsync(
        Guid assignmentId)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.SectionResultApproval);

        EnsureReviewer();

        assignment.IsApprovedBySection = true;
        assignment.IsRejectedBySection = false;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.TechnicalManagerResultApproval;

        _unitOfWork.TestAssignments.Update(assignment);

        await AddReviewAsync(
            assignment,
            TestResultReviewLevel.SectionHead,
            TestResultReviewDecision.Approved,
            null);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectBySectionAsync(
        Guid assignmentId,
        string? reason)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.SectionResultApproval);

        EnsureReviewer();

        assignment.IsApprovedBySection = false;
        assignment.IsRejectedBySection = true;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.AnalystWork;

        _unitOfWork.TestAssignments.Update(assignment);

        await AddReviewAsync(
            assignment,
            TestResultReviewLevel.SectionHead,
            TestResultReviewDecision.Rejected,
            reason);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApprovePanelBySectionAsync(
    Guid sampleId,
    Guid testPanelId)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty)
            return;

        var assignments =
            await _unitOfWork.TestAssignments
                .GetBySampleIdAndTestPanelIdAsync(
                    sampleId,
                    testPanelId);

        if (assignments is null ||
            assignments.Count == 0)
        {
            throw new InvalidOperationException(
                "آزمون‌های Panel موردنظر پیدا نشدند.");
        }

        foreach (var assignment in assignments)
        {
            EnsureStage(
                assignment,
                TestAssignmentWorkflowStage.SectionResultApproval);

            EnsureReviewer();

            assignment.IsApprovedBySection = true;
            assignment.IsRejectedBySection = false;
            assignment.WorkflowStage =
                TestAssignmentWorkflowStage.TechnicalManagerResultApproval;

            _unitOfWork.TestAssignments.Update(assignment);

            await AddReviewAsync(
                assignment,
                TestResultReviewLevel.SectionHead,
                TestResultReviewDecision.Approved,
                null);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectPanelBySectionAsync(
        Guid sampleId,
        Guid testPanelId,
        string? reason)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty)
            return;

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException(
                "علت رد نتیجه باید وارد شود.");

        var assignments =
            await _unitOfWork.TestAssignments
                .GetBySampleIdAndTestPanelIdAsync(
                    sampleId,
                    testPanelId);

        if (assignments is null ||
            assignments.Count == 0)
        {
            throw new InvalidOperationException(
                "آزمون‌های Panel موردنظر پیدا نشدند.");
        }

        foreach (var assignment in assignments)
        {
            EnsureStage(
                assignment,
                TestAssignmentWorkflowStage.SectionResultApproval);

            EnsureReviewer();

            assignment.IsApprovedBySection = false;
            assignment.IsRejectedBySection = true;
            assignment.WorkflowStage =
                TestAssignmentWorkflowStage.AnalystWork;

            _unitOfWork.TestAssignments.Update(assignment);

            await AddReviewAsync(
                assignment,
                TestResultReviewLevel.SectionHead,
                TestResultReviewDecision.Rejected,
                reason.Trim());
        }

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApproveByTechnicalManagerAsync(
        Guid assignmentId)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.TechnicalManagerResultApproval);

        EnsureReviewer();

        assignment.IsApprovedByTechManager = true;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.DirectorResultApproval;

        _unitOfWork.TestAssignments.Update(assignment);

        await AddReviewAsync(
            assignment,
            TestResultReviewLevel.TechnicalManager,
            TestResultReviewDecision.Approved,
            null);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectByTechnicalManagerAsync(
        Guid assignmentId,
        string? reason)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.TechnicalManagerResultApproval);

        EnsureReviewer();

        assignment.IsApprovedByTechManager = false;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.SectionResultApproval;

        _unitOfWork.TestAssignments.Update(assignment);

        await AddReviewAsync(
            assignment,
            TestResultReviewLevel.TechnicalManager,
            TestResultReviewDecision.Rejected,
            reason);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveByDirectorAsync(
        Guid assignmentId)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.DirectorResultApproval);

        EnsureReviewer();

        assignment.IsApprovedByDirector = true;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.Completed;
        assignment.CompletedAt = DateTime.UtcNow;

        _unitOfWork.TestAssignments.Update(assignment);

        await AddReviewAsync(
            assignment,
            TestResultReviewLevel.Director,
            TestResultReviewDecision.Approved,
            null);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectByDirectorAsync(
        Guid assignmentId,
        string? reason)
    {
        var assignment = await GetAssignmentAsync(assignmentId);

        EnsureStage(
            assignment,
            TestAssignmentWorkflowStage.DirectorResultApproval);

        EnsureReviewer();

        assignment.IsApprovedByDirector = false;
        assignment.WorkflowStage =
            TestAssignmentWorkflowStage.TechnicalManagerResultApproval;

        _unitOfWork.TestAssignments.Update(assignment);

        await AddReviewAsync(
            assignment,
            TestResultReviewLevel.Director,
            TestResultReviewDecision.Rejected,
            reason);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<TestAssignment> GetAssignmentAsync(
        Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            throw new InvalidOperationException(
                "شناسه Assignment معتبر نیست.");

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "Assignment پیدا نشد.");

        return assignment;
    }

    private static void EnsureStage(
        TestAssignment assignment,
        TestAssignmentWorkflowStage expectedStage)
    {
        if (assignment.WorkflowStage != expectedStage)
        {
            throw new InvalidOperationException(
                $"عملیات در مرحله فعلی Workflow مجاز نیست. " +
                $"مرحله فعلی: {assignment.WorkflowStage}، " +
                $"مرحله مورد انتظار: {expectedStage}.");
        }
    }

    private void EnsureReviewer()
    {
        var employeeId = _currentUserService.EmployeeId;

        if (!employeeId.HasValue ||
            employeeId.Value == Guid.Empty)
            throw new InvalidOperationException(
                "کاربر جاری مشخص نیست.");
    }

    private async Task AddReviewAsync(
        TestAssignment assignment,
        TestResultReviewLevel level,
        TestResultReviewDecision decision,
        string? reason)
    {
        var employeeId = _currentUserService.EmployeeId;

        if (!employeeId.HasValue ||
            employeeId.Value == Guid.Empty)
            throw new InvalidOperationException(
                "کاربر جاری مشخص نیست.");

        var review = new TestResultReview
        {
            TestAssignmentId = assignment.Id,
            ReviewerEmployeeId = employeeId.Value,
            ReviewLevel = level,
            Decision = decision,
            Reason = reason
        };

        await _unitOfWork.TestResultReviews.AddAsync(review);
    }
}
