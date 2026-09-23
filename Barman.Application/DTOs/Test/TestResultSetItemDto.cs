namespace Barman.Application.DTOs.Test;

public class TestResultSetItemDto
{
    public Guid Id { get; set; }

    public Guid TestResultSetId { get; set; }

    public Guid TestResultDefinitionId { get; set; }

    public int DisplayOrder { get; set; }

    // =========================
    // Result Limits
    // =========================

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    // =========================
    // Result Definition Info
    // =========================

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Unit { get; set; }

    public bool IsRequired { get; set; }

    public int? DecimalPlaces { get; set; }
}