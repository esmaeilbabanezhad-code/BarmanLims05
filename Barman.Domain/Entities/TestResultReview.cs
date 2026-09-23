using Barman.Domain.Common;
using Barman.Domain.Enums;

namespace Barman.Domain.Entities;

public class TestResultReview : BaseEntity
{
    public Guid TestAssignmentId { get; set; }

    public TestAssignment TestAssignment { get; set; } = null!;

    public Guid ReviewerEmployeeId { get; set; }

    public Employee ReviewerEmployee { get; set; } = null!;

    public TestResultReviewLevel ReviewLevel { get; set; }

    public TestResultReviewDecision Decision { get; set; }

    public string? Reason { get; set; }
}
