using Barman.Domain.Common;
using Barman.Domain.Enums;

namespace Barman.Domain.Entities;

public class TestAssignment : BaseEntity
{
    public Guid SampleId { get; set; }

    public Sample Sample { get; set; } = null!;

    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    public Guid? TestPanelId { get; set; }

    public TestPanel? TestPanel { get; set; }

    public Guid? TestResultSetId { get; set; }

    public TestResultSet? TestResultSet { get; set; }

    public Guid? SelectedLimitRuleId { get; set; }

    public TestLimitRule? SelectedLimitRule { get; set; }

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public Guid? TechnicalManagerId { get; set; }

    public Employee? TechnicalManager { get; set; }

    public Guid? TechnicalManagerScopeId { get; set; }

    public TechnicalManagerScope? TechnicalManagerScope { get; set; }

    public Guid? SectionHeadId { get; set; }

    public Guid? AnalystId { get; set; }

    public string? Result { get; set; }

    public string? FinalResult { get; set; }

    public string? Unit { get; set; }

    public string? Comment { get; set; }

    // =========================
    // Result Values
    // =========================

    public ICollection<TestAssignmentResultValue> ResultValues { get; set; }
        = new List<TestAssignmentResultValue>();

    public TestAssignmentWorkflowStage WorkflowStage { get; set; }

    public bool IsApprovedBySection { get; set; }

    public bool IsRejectedBySection { get; set; }

    public bool IsApprovedByTechManager { get; set; }

    public bool IsApprovedByDirector { get; set; }

    public DateTime? CompletedAt { get; set; }
}
