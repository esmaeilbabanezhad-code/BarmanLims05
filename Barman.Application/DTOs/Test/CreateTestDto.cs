namespace Barman.Application.DTOs.Test;

public class CreateTestDto
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? EnglishName { get; set; }

    public string? Unit { get; set; }

    public string? Method { get; set; }

    public string? InstrumentName { get; set; }

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public string? DefaultResult { get; set; }

    public bool IsQuantitative { get; set; }

    public bool IsActiveForReception { get; set; } = true;

    public string? Description { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid? DefaultAnalystId { get; set; }
}
