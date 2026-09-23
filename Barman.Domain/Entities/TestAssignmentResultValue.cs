using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestAssignmentResultValue : BaseEntity
{
    // =========================
    // Test Assignment
    // =========================

    public Guid TestAssignmentId { get; set; }

    public TestAssignment TestAssignment { get; set; } = null!;

    // =========================
    // Result Set Item
    // =========================

    public Guid TestResultSetItemId { get; set; }

    public TestResultSetItem TestResultSetItem { get; set; } = null!;

    // =========================
    // Actual Result
    // =========================

    public string? Value { get; set; }

    // =========================
    // Result Comment
    // =========================

    public string? Comment { get; set; }
}