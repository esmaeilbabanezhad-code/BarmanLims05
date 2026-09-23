namespace Barman.Application.DTOs.Reception;

public class ReceptionTestStatusDto
{
    public Guid AssignmentId { get; set; }

    public Guid SampleId { get; set; }

    public string TestCode { get; set; } = "";

    public string TestName { get; set; } = "";

    public string? DepartmentName { get; set; }

    public string? Unit { get; set; }

    public bool IsQuantitative { get; set; }

    public bool HasResult { get; set; }

    public int WorkflowStage { get; set; }
}