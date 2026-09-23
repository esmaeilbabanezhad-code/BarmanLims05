using Barman.Domain.Common;
using Barman.Domain.Enums;

namespace Barman.Domain.Entities;

public class TestLimitChangeRequest : BaseEntity
{
    // آزمون
    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    // اجرای مشخص آزمون / نمونه
    public Guid? TestAssignmentId { get; set; }

    public TestAssignment? TestAssignment { get; set; }

    // Result Row مشخص در صورت استفاده از ResultSet
    public Guid? TestResultSetItemId { get; set; }

    public TestResultSetItem? TestResultSetItem { get; set; }

    // حدود مجاز فعلی
    public Guid? ReferenceLimitId { get; set; }

    public ReferenceLimit? ReferenceLimit { get; set; }

    // درخواست‌کننده
    public Guid RequestedByEmployeeId { get; set; }

    public Employee RequestedByEmployee { get; set; } = null!;

    // -------------------------
    // LOD
    // -------------------------

    public decimal? CurrentLOD { get; set; }

    public decimal? RequestedLOD { get; set; }

    // -------------------------
    // LOQ
    // -------------------------

    public decimal? CurrentLOQ { get; set; }

    public decimal? RequestedLOQ { get; set; }

    // -------------------------
    // حدود مجاز
    // -------------------------

    public decimal? CurrentMinValue { get; set; }

    public decimal? RequestedMinValue { get; set; }

    public decimal? CurrentMaxValue { get; set; }

    public decimal? RequestedMaxValue { get; set; }

    // -------------------------
    // حدود هشدار
    // -------------------------

    public decimal? CurrentWarningLow { get; set; }

    public decimal? RequestedWarningLow { get; set; }

    public decimal? CurrentWarningHigh { get; set; }

    public decimal? RequestedWarningHigh { get; set; }

    // -------------------------
    // توضیح درخواست
    // -------------------------

    public string? Reason { get; set; }

    // -------------------------
    // وضعیت درخواست
    // -------------------------

    public TestLimitChangeRequestStatus Status { get; set; }
        = TestLimitChangeRequestStatus.Pending;

    // -------------------------
    // تأیید مسئول بخش
    // -------------------------

    public Guid? ApprovedByEmployeeId { get; set; }

    public Employee? ApprovedByEmployee { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public string? ApprovalComment { get; set; }

    // -------------------------
    // تأیید مسئول فنی
    // -------------------------

    public TestLimitChangeRequestStatus TechnicalManagerStatus { get; set; }
        = TestLimitChangeRequestStatus.Pending;

    public Guid? TechnicalManagerApprovedByEmployeeId { get; set; }

    public Employee? TechnicalManagerApprovedByEmployee { get; set; }

    public DateTimeOffset? TechnicalManagerApprovedAt { get; set; }

    public string? TechnicalManagerApprovalComment { get; set; }
}