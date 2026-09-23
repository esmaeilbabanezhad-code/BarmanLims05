namespace Barman.Application.DTOs.Test;

public class TestResultSetDto
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    // =========================
    // Scope
    // =========================

    public Guid? CustomerId { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid? StandardSampleId { get; set; }

    // =========================
    // Identity
    // =========================

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    // =========================
    // Priority
    // =========================

    public int Priority { get; set; } = 100;

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; } = true;
}