using Barman.Application.DTOs.Reception;
using Barman.Application.DTOs.Test;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;
using Barman.Domain.Enums;

namespace Barman.Application.Services;

public class TestAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestAssignmentWorkflowService _workflowService;

    public TestAssignmentService(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    ITestAssignmentWorkflowService workflowService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _workflowService = workflowService;
    }

    public ReferenceLimit? GetApplicableReferenceLimit(TestAssignment assignment)
    {
        if (assignment is null)
            return null;

        if (assignment.Test is null)
            return null;

        var limits = assignment.Test.ReferenceLimits;

        if (limits is null || !limits.Any())
            return null;

        var today = DateTime.UtcNow.Date;

        var validLimits = limits
            .Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom.Value.Date <= today) &&
                (!x.ValidTo.HasValue || x.ValidTo.Value.Date >= today))
            .ToList();

        if (!validLimits.Any())
            return null;

        var matrixId = assignment.Sample?.MatrixId;
        var sampleName = assignment.Sample?.SampleName?.Trim();

        var productAndMatrixLimit = validLimits
            .Where(x =>
                x.MatrixId.HasValue &&
                matrixId.HasValue &&
                x.MatrixId.Value == matrixId.Value &&
                !string.IsNullOrWhiteSpace(x.ProductName) &&
                !string.IsNullOrWhiteSpace(sampleName) &&
                string.Equals(
                    x.ProductName.Trim(),
                    sampleName,
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault();

        if (productAndMatrixLimit is not null)
            return productAndMatrixLimit;

        var matrixLimit = validLimits
            .Where(x =>
                x.MatrixId.HasValue &&
                matrixId.HasValue &&
                x.MatrixId.Value == matrixId.Value &&
                string.IsNullOrWhiteSpace(x.ProductName))
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault();

        if (matrixLimit is not null)
            return matrixLimit;

        return validLimits
            .Where(x =>
                !x.MatrixId.HasValue &&
                string.IsNullOrWhiteSpace(x.ProductName))
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault();
    }

    public TestLimitRule? GetApplicableLimitRule(TestAssignment assignment)
    {
        if (assignment is null)
            return null;

        if (assignment.SelectedLimitRule is not null)
            return assignment.SelectedLimitRule;

        return null;
    }
    public async Task<TestLimitRule?> ResolveApplicableLimitRuleAsync(
    TestAssignment assignment)
    {
        if (assignment is null)
            return null;

        if (assignment.TestId == Guid.Empty)
            return null;

        // اگر Rule قبلاً به‌صورت مشخص برای Assignment انتخاب شده،
        // همان Rule باید مرجع اصلی باشد.
        if (assignment.SelectedLimitRule is not null)
            return assignment.SelectedLimitRule;

        var customerId = assignment.Sample?.Reception?.CustomerId;
        var matrixId = assignment.Sample?.MatrixId;
        var sampleCategoryId = assignment.Sample?.SampleCategoryId;

        var rules = await _unitOfWork.TestLimitRules
            .GetApplicableAsync(
                assignment.TestId,
                customerId,
                matrixId,
                sampleCategoryId);

        return rules.FirstOrDefault();
    }
    public bool IsResultOutOfLimit(TestAssignment assignment)
    {
        if (assignment is null)
            return false;

        if (string.IsNullOrWhiteSpace(assignment.Result))
            return false;

        var rule = GetApplicableLimitRule(assignment);

        if (rule is not null)
        {
            if (!decimal.TryParse(assignment.Result, out var resultValue))
                return false;

            return rule.LimitType switch
            {
                LimitType.Maximum =>
                    rule.UpperValue.HasValue &&
                    resultValue > rule.UpperValue.Value,

                LimitType.Minimum =>
                    rule.LowerValue.HasValue &&
                    resultValue < rule.LowerValue.Value,

                LimitType.Range =>
                    (rule.LowerValue.HasValue &&
                     (rule.LowerInclusive
                         ? resultValue < rule.LowerValue.Value
                         : resultValue <= rule.LowerValue.Value))
                    ||
                    (rule.UpperValue.HasValue &&
                     (rule.UpperInclusive
                         ? resultValue > rule.UpperValue.Value
                         : resultValue >= rule.UpperValue.Value)),

                LimitType.Exact =>
                    rule.LowerValue.HasValue &&
                    resultValue != rule.LowerValue.Value,

                _ => false
            };
        }

        var referenceLimit = GetApplicableReferenceLimit(assignment);

        if (referenceLimit is null)
            return false;

        if (!decimal.TryParse(
                assignment.Result,
                out var referenceResultValue))
            return false;

        if (referenceLimit.MinValue.HasValue &&
            referenceResultValue < referenceLimit.MinValue.Value)
        {
            return true;
        }

        if (referenceLimit.MaxValue.HasValue &&
            referenceResultValue > referenceLimit.MaxValue.Value)
        {
            return true;
        }

        return false;
    }
    public async Task<ResultSetStatus> GetResultSetStatusAsync(
    TestAssignment assignment)
    {
        if (assignment is null)
            return ResultSetStatus.NoResult;

        if (!assignment.TestResultSetId.HasValue ||
            assignment.TestResultSetId.Value == Guid.Empty)
        {
            return string.IsNullOrWhiteSpace(assignment.Result)
                ? ResultSetStatus.NoResult
                : ResultSetStatus.Normal;
        }

        var items =
            await _unitOfWork.TestResultSetItems
                .GetByResultSetIdAsync(
                    assignment.TestResultSetId.Value);

        var activeItems =
            items
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

        if (activeItems.Count == 0)
            return ResultSetStatus.NoResult;

        var values =
            await _unitOfWork.TestAssignmentResultValues
                .GetByAssignmentIdAsync(
                    assignment.Id);

        var requiredItems =
            activeItems
                .Where(x => x.TestResultDefinition.IsRequired)
                .ToList();

        var enteredRequiredCount =
            requiredItems.Count(item =>
                values.Any(value =>
                    value.TestResultSetItemId == item.Id &&
                    !string.IsNullOrWhiteSpace(value.Value)));

        if (enteredRequiredCount == 0)
            return ResultSetStatus.NoResult;

        if (enteredRequiredCount < requiredItems.Count)
            return ResultSetStatus.Incomplete;

        bool hasOutOfLimit = false;
        bool hasAnyLimit = false;

        foreach (var item in activeItems)
        {
            var value =
                values.FirstOrDefault(x =>
                    x.TestResultSetItemId == item.Id);

            if (value is null ||
                string.IsNullOrWhiteSpace(value.Value))
                continue;

            if (!decimal.TryParse(
                    value.Value,
                    out var numericValue))
                continue;

            if (item.MinValue.HasValue ||
                item.MaxValue.HasValue)
            {
                hasAnyLimit = true;

                if (item.MinValue.HasValue &&
                    numericValue < item.MinValue.Value)
                {
                    hasOutOfLimit = true;
                    break;
                }

                if (item.MaxValue.HasValue &&
                    numericValue > item.MaxValue.Value)
                {
                    hasOutOfLimit = true;
                    break;
                }
            }
        }

        if (hasOutOfLimit)
            return ResultSetStatus.OutOfLimit;

        if (!hasAnyLimit)
            return ResultSetStatus.NoApplicableLimit;

        return ResultSetStatus.Normal;
    }
    public async Task<List<TestAssignment>> GetBySampleIdAsync(Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetBySampleIdAsync(sampleId);
    }
    public async Task<List<TestAssignment>> GetBySampleIdAndTestPanelIdAsync(
    Guid sampleId,
    Guid testPanelId)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetBySampleIdAndTestPanelIdAsync(
                sampleId,
                testPanelId);
    }
    public async Task<List<TestAssignment>> GetByReceptionIdAsync(Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetByReceptionIdAsync(receptionId);
    }
    public async Task<List<ReceptionTestStatusDto>> GetReceptionTestStatusesAsync(
    Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return new List<ReceptionTestStatusDto>();

        return await _unitOfWork.TestAssignments
            .GetReceptionTestStatusesAsync(receptionId);
    }
    public async Task<List<TestAssignment>> GetPendingForTechnicalManagerAsync(
    Guid technicalManagerId)
    {
        if (technicalManagerId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingForTechnicalManagerAsync(technicalManagerId);
    }

    public async Task<List<TestAssignment>> GetPendingForSectionHeadAsync(
        Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingForSectionHeadAsync(departmentId);
    }

    public async Task<List<TestAssignment>> GetPendingResultApprovalForSectionHeadAsync(
        Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingResultApprovalForSectionHeadAsync(departmentId);
    }
    public async Task<List<ResultSetStatusDto>> GetResultSetStatusesAsync(
    IReadOnlyCollection<TestAssignment> assignments)
    {
        if (assignments is null || assignments.Count == 0)
            return new List<ResultSetStatusDto>();

        var resultSetAssignments =
            assignments
                .Where(x =>
                    x.TestResultSetId.HasValue &&
                    x.TestResultSetId.Value != Guid.Empty)
                .ToList();

        if (resultSetAssignments.Count == 0)
            return new List<ResultSetStatusDto>();

        var assignmentIds =
            resultSetAssignments
                .Select(x => x.Id)
                .Distinct()
                .ToList();

        var resultValues =
            await _unitOfWork.TestAssignmentResultValues
                .GetByAssignmentIdsAsync(assignmentIds);

        var result = new List<ResultSetStatusDto>();

        foreach (var assignment in resultSetAssignments)
        {
            var items =
                assignment.TestResultSet?.Items?
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList()
                ?? new List<TestResultSetItem>();

            var requiredItems =
                items
                    .Where(x => x.TestResultDefinition.IsRequired)
                    .ToList();

            var assignmentValues =
                resultValues
                    .Where(x =>
                        x.TestAssignmentId == assignment.Id)
                    .ToList();

            var resultCount =
                requiredItems.Count(item =>
                    assignmentValues.Any(value =>
                        value.TestResultSetItemId == item.Id &&
                        !string.IsNullOrWhiteSpace(value.Value)));

            var outOfLimitCount = 0;
            var hasAnyLimit = false;

            foreach (var item in items)
            {
                var value =
                    assignmentValues.FirstOrDefault(x =>
                        x.TestResultSetItemId == item.Id);

                if (value is null ||
                    string.IsNullOrWhiteSpace(value.Value))
                    continue;

                if (!decimal.TryParse(
                        value.Value,
                        out var numericValue))
                    continue;

                if (item.MinValue.HasValue ||
                    item.MaxValue.HasValue)
                {
                    hasAnyLimit = true;

                    if (item.MinValue.HasValue &&
                        numericValue < item.MinValue.Value)
                    {
                        outOfLimitCount++;
                        continue;
                    }

                    if (item.MaxValue.HasValue &&
                        numericValue > item.MaxValue.Value)
                    {
                        outOfLimitCount++;
                    }
                }
            }

            result.Add(new ResultSetStatusDto
            {
                AssignmentId = assignment.Id,
                ResultCount = resultCount,
                RequiredResultCount = requiredItems.Count,
                OutOfLimitCount = outOfLimitCount,
                HasAnyLimit = hasAnyLimit
            });
        }

        return result;
    }
    public async Task<List<TestAssignment>> GetPendingResultApprovalForTechnicalManagerAsync()
    {
        return await _unitOfWork.TestAssignments
            .GetPendingResultApprovalForTechnicalManagerAsync();
    }

    public async Task<List<TestAssignment>> GetPendingResultApprovalForDirectorAsync()
    {
        return await _unitOfWork.TestAssignments
            .GetPendingResultApprovalForDirectorAsync();
    }

    public async Task<List<TestAssignment>> GetPendingForAnalystAsync(
        Guid analystId)
    {
        if (analystId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingForAnalystAsync(analystId);
    }

    public async Task<List<TestAssignment>> GetPendingForAnalystBySampleAsync(
    Guid analystId,
    Guid sampleId)
    {
        if (analystId == Guid.Empty || sampleId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingForAnalystBySampleAsync(
                analystId,
                sampleId);
    }

    public async Task<TestResultReview?> GetLatestSectionRejectionAsync(
    Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            return null;

        var reviews =
            await _unitOfWork.TestResultReviews
                .GetByAssignmentIdAsync(assignmentId);

        return reviews
            .Where(x =>
                x.ReviewLevel == TestResultReviewLevel.SectionHead &&
                x.Decision == TestResultReviewDecision.Rejected &&
                !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();
    }
    public async Task<List<TestResultReview>> GetResultReviewHistoryAsync(
    Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            return new List<TestResultReview>();

        return await _unitOfWork.TestResultReviews
            .GetByAssignmentIdAsync(assignmentId);
    }
    public async Task SaveResultSetResultsAsync(
    Guid assignmentId,
    IReadOnlyCollection<(
        Guid ItemId,
        string? Value,
        string? Comment,
        decimal? RequestedLOD,
        decimal? RequestedLOQ,
        decimal? RequestedMinValue,
        decimal? RequestedMaxValue,
        string? LimitChangeReason
    )> results,
    string? comment)
    {
        if (assignmentId == Guid.Empty)
            return;

        if (results is null || results.Count == 0)
            throw new InvalidOperationException(
                "هیچ نتیجه‌ای برای ثبت وجود ندارد.");

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        if (!assignment.TestResultSetId.HasValue)
            throw new InvalidOperationException(
                "برای این آزمون Result Set تعریف نشده است.");

        var items =
            await _unitOfWork.TestResultSetItems
                .GetByResultSetIdAsync(
                    assignment.TestResultSetId.Value);

        var activeItems =
            items
                .Where(x => !x.IsDeleted)
                .ToList();

        if (activeItems.Count == 0)
            throw new InvalidOperationException(
                "برای Result Set این آزمون هیچ جزء نتیجه‌ای تعریف نشده است.");

        // =========================================================
        // Required validation
        // =========================================================

        foreach (var item in activeItems)
        {
            if (!item.TestResultDefinition.IsRequired)
                continue;

            var submitted =
                results.FirstOrDefault(
                    x => x.ItemId == item.Id);

            if (submitted.ItemId == Guid.Empty ||
                string.IsNullOrWhiteSpace(submitted.Value))
            {
                throw new InvalidOperationException(
                    $"نتیجه «{item.TestResultDefinition.Name}» وارد نشده است.");
            }
        }

        // =========================================================
        // Validate submitted items + save results
        // =========================================================

        foreach (var result in results)
        {
            if (result.ItemId == Guid.Empty)
                continue;

            var item =
                activeItems.FirstOrDefault(
                    x => x.Id == result.ItemId);

            if (item is null)
            {
                throw new InvalidOperationException(
                    "یکی از اجزای نتیجه متعلق به Result Set این آزمون نیست.");
            }

            var existing =
                await _unitOfWork.TestAssignmentResultValues
                    .GetByAssignmentAndItemAsync(
                        assignmentId,
                        result.ItemId);

            if (existing is null)
            {
                existing = new TestAssignmentResultValue
                {
                    Id = Guid.NewGuid(),
                    TestAssignmentId = assignmentId,
                    TestResultSetItemId = result.ItemId,
                    Value = string.IsNullOrWhiteSpace(result.Value)
                        ? null
                        : result.Value.Trim(),
                    Comment = string.IsNullOrWhiteSpace(result.Comment)
                        ? null
                        : result.Comment.Trim(),
                    IsDeleted = false,
                    IsActive = true
                };

                await _unitOfWork.TestAssignmentResultValues
                    .AddAsync(existing);
            }
            else
            {
                existing.Value =
                    string.IsNullOrWhiteSpace(result.Value)
                        ? null
                        : result.Value.Trim();

                existing.Comment =
                    string.IsNullOrWhiteSpace(result.Comment)
                        ? null
                        : result.Comment.Trim();

                existing.IsDeleted = false;
                existing.IsActive = true;

                _unitOfWork.TestAssignmentResultValues
                    .Update(existing);
            }

            // =====================================================
            // ResultSet Item Limit Change Request
            // =====================================================

            bool hasLimitProposal =
                result.RequestedLOD.HasValue ||
                result.RequestedLOQ.HasValue ||
                result.RequestedMinValue.HasValue ||
                result.RequestedMaxValue.HasValue;

            if (!hasLimitProposal)
                continue;

            bool lodChanged =
                item.LOD != result.RequestedLOD;

            bool loqChanged =
                item.LOQ != result.RequestedLOQ;

            bool minChanged =
                item.MinValue != result.RequestedMinValue;

            bool maxChanged =
                item.MaxValue != result.RequestedMaxValue;

            if (!lodChanged &&
                !loqChanged &&
                !minChanged &&
                !maxChanged)
            {
                throw new InvalidOperationException(
                    $"مقادیر پیشنهادی جزء «{item.TestResultDefinition.Name}» با مقادیر فعلی تفاوتی ندارند.");
            }

            if (string.IsNullOrWhiteSpace(
                    result.LimitChangeReason))
            {
                throw new InvalidOperationException(
                    $"برای پیشنهاد تغییر حدود جزء «{item.TestResultDefinition.Name}»، وارد کردن علت پیشنهاد الزامی است.");
            }

            var requestedByEmployeeId =
                _currentUserService.EmployeeId
                ?? throw new InvalidOperationException(
                    "کاربر جاری مشخص نیست.");

            var request =
                new TestLimitChangeRequest
                {
                    TestAssignmentId =
                        assignment.Id,

                    TestId =
                        assignment.TestId,

                    TestResultSetItemId =
                        item.Id,

                    RequestedByEmployeeId =
                        requestedByEmployeeId,

                    CurrentLOD =
                        item.LOD,

                    CurrentLOQ =
                        item.LOQ,

                    CurrentMinValue =
                        item.MinValue,

                    CurrentMaxValue =
                        item.MaxValue,

                    RequestedLOD =
                        result.RequestedLOD,

                    RequestedLOQ =
                        result.RequestedLOQ,

                    RequestedMinValue =
                        result.RequestedMinValue,

                    RequestedMaxValue =
                        result.RequestedMaxValue,

                    CurrentWarningLow = null,
                    RequestedWarningLow = null,

                    CurrentWarningHigh = null,
                    RequestedWarningHigh = null,

                    Reason =
                        result.LimitChangeReason.Trim(),

                    Status =
                        TestLimitChangeRequestStatus.Pending
                };

            await _unitOfWork.TestLimitChangeRequests
                .AddAsync(request);
        }

        // =========================================================
        // Assignment
        // =========================================================

        assignment.Comment = comment;
        assignment.IsRejectedBySection = false;

        _unitOfWork.TestAssignments.Update(assignment);

        // =========================================================
        // Workflow
        // =========================================================

        await _workflowService
            .SubmitResultSetAsync(assignment.Id);

        // SubmitResultSetAsync خودش SaveChanges انجام می‌دهد.
    }
    public async Task SaveResultAsync(
    Guid assignmentId,
    string? result,
    string? comment,
    decimal? requestedLOD,
    decimal? requestedLOQ,
    decimal? requestedMinValue,
    decimal? requestedMaxValue,
    string? limitChangeReason)
    {
        if (assignmentId == Guid.Empty)
            return;

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        var resolvedRule =
            await ResolveApplicableLimitRuleAsync(assignment);

        if (resolvedRule is not null &&
            assignment.SelectedLimitRuleId != resolvedRule.Id)
        {
            assignment.SelectedLimitRuleId = resolvedRule.Id;
        }

        assignment.SelectedLimitRule = resolvedRule;
        assignment.Result = result;
        assignment.IsRejectedBySection = false;
        assignment.Comment = comment;
        

        _unitOfWork.TestAssignments.Update(assignment);

        // =========================================================
        // درخواست تغییر حدود پیشنهادی کارشناس
        // =========================================================

        bool hasLimitChangeRequest =
            requestedLOD.HasValue ||
            requestedLOQ.HasValue ||
            requestedMinValue.HasValue ||
            requestedMaxValue.HasValue;

        if (hasLimitChangeRequest)
        {
            if (string.IsNullOrWhiteSpace(limitChangeReason))
            {
                throw new InvalidOperationException(
                    "برای ثبت پیشنهاد تغییر حدود، وارد کردن علت پیشنهاد الزامی است.");
            }

            var referenceLimit =
                GetApplicableReferenceLimit(assignment);

            bool lodChanged =
                assignment.Test?.LOD != requestedLOD;

            bool loqChanged =
                assignment.Test?.LOQ != requestedLOQ;

            bool minChanged =
                referenceLimit?.MinValue != requestedMinValue;

            bool maxChanged =
                referenceLimit?.MaxValue != requestedMaxValue;

            if (!lodChanged &&
                !loqChanged &&
                !minChanged &&
                !maxChanged)
            {
                throw new InvalidOperationException(
                    "مقادیر پیشنهادی با مقادیر فعلی تفاوتی ندارند.");
            }

            var request = new TestLimitChangeRequest
            {
                TestAssignmentId = assignment.Id,
                TestId = assignment.TestId,

                ReferenceLimitId =
                    referenceLimit?.Id,

                RequestedByEmployeeId =
                    _currentUserService.EmployeeId
                    ?? throw new InvalidOperationException(
                        "کاربر جاری مشخص نیست."),

                CurrentLOD =
                    assignment.Test?.LOD,

                CurrentLOQ =
                    assignment.Test?.LOQ,

                CurrentMinValue =
                    referenceLimit?.MinValue,

                CurrentMaxValue =
                    referenceLimit?.MaxValue,

                CurrentWarningLow =
                    referenceLimit?.WarningLow,

                CurrentWarningHigh =
                    referenceLimit?.WarningHigh,

                RequestedLOD =
                    requestedLOD,

                RequestedLOQ =
                    requestedLOQ,

                RequestedMinValue =
                    requestedMinValue,

                RequestedMaxValue =
                    requestedMaxValue,

                RequestedWarningLow = null,

                RequestedWarningHigh = null,

                Reason =
                    limitChangeReason.Trim(),

                Status =
                    TestLimitChangeRequestStatus.Pending
            };

            await _unitOfWork.TestLimitChangeRequests
                .AddAsync(request);
        }
        await _workflowService
            .SubmitResultAsync(
                assignment.Id,
                result ?? "");
        // =========================================================
        // یک ذخیره نهایی برای نتیجه + پیشنهاد تغییر حدود
        // =========================================================

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveResultBySectionAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            return;

        await _workflowService
            .ApproveBySectionAsync(assignmentId);
    }
    public async Task ApprovePanelResultsBySectionAsync(
    Guid sampleId,
    Guid testPanelId)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty)
            return;

        await _workflowService
            .ApprovePanelBySectionAsync(
                sampleId,
                testPanelId);
    }
    public async Task RejectPanelResultsBySectionAsync(
    Guid sampleId,
    Guid testPanelId,
    string? reason)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty)
            return;

        await _workflowService
            .RejectPanelBySectionAsync(
                sampleId,
                testPanelId,
                reason);
    }
    public async Task RejectResultBySectionAsync(
    Guid assignmentId,
    string? reason)
    {
        if (assignmentId == Guid.Empty)
            return;

        await _workflowService
            .RejectBySectionAsync(
                assignmentId,
                reason);
    }

    public async Task ApproveResultByTechManagerAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            return;

        await _workflowService
            .ApproveByTechnicalManagerAsync(assignmentId);
    }
    public async Task RejectResultByTechnicalManagerAsync(
    Guid assignmentId,
    string? reason)
    {
        if (assignmentId == Guid.Empty)
            return;

        await _workflowService
            .RejectByTechnicalManagerAsync(
                assignmentId,
                reason);
    }
    public async Task ApproveResultByTechnicalManagerAsync(Guid assignmentId)
    {
        await ApproveResultByTechManagerAsync(assignmentId);
    }

    public async Task ApproveResultByDirectorAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            return;

        await _workflowService
            .ApproveByDirectorAsync(assignmentId);
    }
    public async Task RejectResultByDirectorAsync(
        Guid assignmentId,
        string? reason)
    {
        if (assignmentId == Guid.Empty)
            return;

        await _workflowService
            .RejectByDirectorAsync(
                assignmentId,
                reason);
    }
    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _unitOfWork.Departments.GetAllAsync();
    }

    public async Task SetDepartmentAsync(
    Guid assignmentId,
    Guid? departmentId)
    {
        if (assignmentId == Guid.Empty ||
            !departmentId.HasValue ||
            departmentId.Value == Guid.Empty)
            return;

        await _workflowService
            .AssignToSectionAsync(
                assignmentId,
                departmentId.Value);
    }

    public async Task AssignToDepartmentAsync(
    Guid assignmentId,
    Guid? departmentId)
    {
        await SetDepartmentAsync(assignmentId, departmentId);
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

        await _workflowService
            .UpdateAssignmentDepartmentAsync(
                assignmentId,
                departmentId);
    }

    public async Task AssignToDefaultDepartmentsAsync(
    List<Guid> assignmentIds)
    {
        if (assignmentIds == null ||
            assignmentIds.Count == 0)
            return;

        foreach (var assignmentId in assignmentIds.Distinct())
        {
            if (assignmentId == Guid.Empty)
                continue;

            var assignment =
                await _unitOfWork.TestAssignments
                    .GetByIdAsync(assignmentId);

            if (assignment is null)
                continue;

            if (assignment.Test is null ||
                !assignment.Test.DepartmentId.HasValue)
                continue;

            await _workflowService
                .AssignToSectionAsync(
                    assignment.Id,
                    assignment.Test.DepartmentId.Value);
        }
    }
    public async Task AssignTestPanelToDepartmentAsync(
    Guid sampleId,
    Guid testPanelId,
    Guid departmentId)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty ||
            departmentId == Guid.Empty)
            return;

        await _workflowService
            .AssignPanelToSectionAsync(
                sampleId,
                testPanelId,
                departmentId);
    }
    public async Task AssignToAnalystAsync(
    Guid assignmentId,
    Guid? analystId)
    {
        if (assignmentId == Guid.Empty)
            return;

        if (!analystId.HasValue || analystId.Value == Guid.Empty)
            throw new InvalidOperationException(
                "کارشناس مشخص نشده است.");

        await _workflowService
            .AssignToAnalystAsync(
                assignmentId,
                analystId.Value);
    }
    public async Task SubmitSelectedAssignmentsToSectionAsync(
    List<Guid> assignmentIds)
    {
        if (assignmentIds is null ||
            assignmentIds.Count == 0)
            return;

        await _workflowService
            .SubmitSelectedAssignmentsToSectionAsync(
                assignmentIds);
    }
    public async Task ReturnSelectedAssignmentsToTechnicalManagerAsync(
    List<Guid> assignmentIds)
    {
        if (assignmentIds is null ||
            assignmentIds.Count == 0)
            return;

        await _workflowService
            .ReturnSelectedAssignmentsToTechnicalManagerAsync(
                assignmentIds);
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

        await _workflowService
            .RequestReceptionCorrectionAsync(
                receptionId,
                sampleId,
                reason);
    }
    
    public async Task<List<Employee>> GetAnalystsByDepartmentAsync(
        Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<Employee>();

        return await _unitOfWork.Employees
            .GetByDepartmentAsync(departmentId);
    }
    public async Task AssignToAnalystsAsync(
    List<Guid> assignmentIds,
    Guid analystId)
    {
        if (assignmentIds == null ||
            assignmentIds.Count == 0)
            return;

        if (analystId == Guid.Empty)
            throw new InvalidOperationException(
                "کارشناس مشخص نشده است.");

        foreach (var assignmentId in assignmentIds)
        {
            await _workflowService
                .AssignToAnalystAsync(
                    assignmentId,
                    analystId);
        }
    }
}
