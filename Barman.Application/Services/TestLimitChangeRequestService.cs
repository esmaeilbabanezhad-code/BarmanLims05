using Barman.Application.Interfaces;
using Barman.Domain.Entities;
using Barman.Domain.Enums;

namespace Barman.Application.Services;

public class TestLimitChangeRequestService
    : ITestLimitChangeRequestService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestLimitChangeRequestService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // =========================================================
    // Create Request
    // =========================================================

    public async Task<TestLimitChangeRequest> CreateRequestAsync(
        Guid testId,
        Guid requestedByEmployeeId,
        Guid? referenceLimitId,
        decimal? requestedLOD,
        decimal? requestedLOQ,
        decimal? requestedMinValue,
        decimal? requestedMaxValue,
        decimal? requestedWarningLow,
        decimal? requestedWarningHigh,
        string? reason)
    {
        if (testId == Guid.Empty)
            throw new InvalidOperationException(
                "آزمون مشخص نشده است.");

        if (requestedByEmployeeId == Guid.Empty)
            throw new InvalidOperationException(
                "کاربر درخواست‌کننده مشخص نشده است.");

        var test = await _unitOfWork.Tests
            .GetByIdAsync(testId);

        if (test is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        ReferenceLimit? referenceLimit = null;

        if (referenceLimitId.HasValue &&
            referenceLimitId.Value != Guid.Empty)
        {
            referenceLimit = await _unitOfWork
                .ReferenceLimits
                .GetByIdAsync(referenceLimitId.Value);

            if (referenceLimit is null)
                throw new InvalidOperationException(
                    "حد مجاز موردنظر پیدا نشد.");
        }

        // ---------------------------------------------------------
        // جلوگیری از ثبت درخواست بی‌معنی
        // ---------------------------------------------------------

        bool lodChanged =
            test.LOD != requestedLOD;

        bool loqChanged =
            test.LOQ != requestedLOQ;

        bool minChanged =
            referenceLimit?.MinValue != requestedMinValue;

        bool maxChanged =
            referenceLimit?.MaxValue != requestedMaxValue;

        bool warningLowChanged =
            referenceLimit?.WarningLow != requestedWarningLow;

        bool warningHighChanged =
            referenceLimit?.WarningHigh != requestedWarningHigh;

        if (!lodChanged &&
            !loqChanged &&
            !minChanged &&
            !maxChanged &&
            !warningLowChanged &&
            !warningHighChanged)
        {
            throw new InvalidOperationException(
                "هیچ تغییری نسبت به مقادیر فعلی ایجاد نشده است.");
        }

        var request = new TestLimitChangeRequest
        {
            TestId = testId,

            ReferenceLimitId =
                referenceLimit?.Id,

            RequestedByEmployeeId =
                requestedByEmployeeId,

            // مقادیر فعلی
            CurrentLOD =
                test.LOD,

            CurrentLOQ =
                test.LOQ,

            CurrentMinValue =
                referenceLimit?.MinValue,

            CurrentMaxValue =
                referenceLimit?.MaxValue,

            CurrentWarningLow =
                referenceLimit?.WarningLow,

            CurrentWarningHigh =
                referenceLimit?.WarningHigh,

            // مقادیر پیشنهادی
            RequestedLOD =
                requestedLOD,

            RequestedLOQ =
                requestedLOQ,

            RequestedMinValue =
                requestedMinValue,

            RequestedMaxValue =
                requestedMaxValue,

            RequestedWarningLow =
                requestedWarningLow,

            RequestedWarningHigh =
                requestedWarningHigh,

            Reason =
                string.IsNullOrWhiteSpace(reason)
                    ? null
                    : reason.Trim(),

            Status =
                TestLimitChangeRequestStatus.Pending
        };

        await _unitOfWork
            .TestLimitChangeRequests
            .AddAsync(request);

        await _unitOfWork.SaveChangesAsync();

        return request;
    }


    // =========================================================
    // Get Pending Requests For Section Head
    // =========================================================

    public async Task<List<TestLimitChangeRequest>>
        GetPendingForSectionHeadAsync(
            Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestLimitChangeRequest>();

        return await _unitOfWork
            .TestLimitChangeRequests
            .GetPendingForSectionHeadAsync(
                departmentId);
    }


    // =========================================================
    // Approve
    // =========================================================

    public async Task ApproveAsync(
        Guid requestId,
        Guid approvedByEmployeeId,
        string? approvalComment)
    {
        if (requestId == Guid.Empty)
            throw new InvalidOperationException(
                "درخواست مشخص نشده است.");

        if (approvedByEmployeeId == Guid.Empty)
            throw new InvalidOperationException(
                "مسئول تأییدکننده مشخص نشده است.");

        var request = await _unitOfWork
            .TestLimitChangeRequests
            .GetByIdAsync(requestId);

        if (request is null)
            throw new InvalidOperationException(
                "درخواست تغییر پیدا نشد.");

        if (request.Status !=
            TestLimitChangeRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "این درخواست قبلاً بررسی شده است.");
        }

        var test = await _unitOfWork.Tests
            .GetByIdAsync(request.TestId);

        if (test is null)
            throw new InvalidOperationException(
                "آزمون مربوط به درخواست پیدا نشد.");

        // ---------------------------------------------------------
        // اعمال LOD و LOQ
        // ---------------------------------------------------------

        if (request.RequestedLOD.HasValue)
        {
            test.LOD = request.RequestedLOD;
        }

        if (request.RequestedLOQ.HasValue)
        {
            test.LOQ = request.RequestedLOQ;
        }


        // ---------------------------------------------------------
        // اعمال حدود مجاز و هشدار
        // ---------------------------------------------------------

        if (request.ReferenceLimitId.HasValue)
        {
            var referenceLimit =
                await _unitOfWork
                    .ReferenceLimits
                    .GetByIdAsync(
                        request.ReferenceLimitId.Value);

            if (referenceLimit is null)
                throw new InvalidOperationException(
                    "رکورد حدود مجاز مربوط به درخواست پیدا نشد.");

            if (request.RequestedMinValue.HasValue)
            {
                referenceLimit.MinValue =
                    request.RequestedMinValue;
            }

            if (request.RequestedMaxValue.HasValue)
            {
                referenceLimit.MaxValue =
                    request.RequestedMaxValue;
            }

           

            _unitOfWork
                .ReferenceLimits
                .Update(referenceLimit);
        }


        // ---------------------------------------------------------
        // ثبت تأیید
        // ---------------------------------------------------------

        request.Status =
            TestLimitChangeRequestStatus.Approved;

        request.ApprovedByEmployeeId =
            approvedByEmployeeId;

        request.ApprovedAt =
            DateTimeOffset.UtcNow;

        request.ApprovalComment =
            string.IsNullOrWhiteSpace(approvalComment)
                ? null
                : approvalComment.Trim();

        _unitOfWork
            .TestLimitChangeRequests
            .Update(request);

        await _unitOfWork.SaveChangesAsync();
    }


    // =========================================================
    // Reject
    // =========================================================

    public async Task RejectAsync(
        Guid requestId,
        Guid approvedByEmployeeId,
        string? approvalComment)
    {
        if (requestId == Guid.Empty)
            throw new InvalidOperationException(
                "درخواست مشخص نشده است.");

        if (approvedByEmployeeId == Guid.Empty)
            throw new InvalidOperationException(
                "مسئول بررسی‌کننده مشخص نشده است.");

        var request = await _unitOfWork
            .TestLimitChangeRequests
            .GetByIdAsync(requestId);

        if (request is null)
            throw new InvalidOperationException(
                "درخواست تغییر پیدا نشد.");

        if (request.Status !=
            TestLimitChangeRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "این درخواست قبلاً بررسی شده است.");
        }

        // ---------------------------------------------------------
        // در حالت Reject هیچ تغییری در Test یا ReferenceLimit
        // اعمال نمی‌شود.
        // ---------------------------------------------------------

        request.Status =
            TestLimitChangeRequestStatus.Rejected;

        request.ApprovedByEmployeeId =
            approvedByEmployeeId;

        request.ApprovedAt =
            DateTimeOffset.UtcNow;

        request.ApprovalComment =
            string.IsNullOrWhiteSpace(approvalComment)
                ? null
                : approvalComment.Trim();

        _unitOfWork
            .TestLimitChangeRequests
            .Update(request);

        await _unitOfWork.SaveChangesAsync();
    }
}