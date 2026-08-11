using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestAssignment : BaseEntity
{
    public Guid SampleId { get; set; }

    public Sample Sample { get; set; } = null!;

    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public Guid? SectionHeadId { get; set; }

    public Guid? AnalystId { get; set; }

    public string? Result { get; set; }

    public string? FinalResult { get; set; }

    public string? Unit { get; set; }

    public string? Comment { get; set; }

    public bool IsApprovedBySection { get; set; }

    public bool IsApprovedByTechManager { get; set; }

    public bool IsApprovedByDirector { get; set; }

    public DateTime? CompletedAt { get; set; }
}